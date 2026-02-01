using System;
using System.Linq;
using System.Data.SqlClient;
using NLog;

namespace AutomataExistencias.Application
{
    public class RecoveryService : IRecoveryService
    {
        // Only keep injected dependencies that are actually used by the recovery logic
        private readonly Domain.Aldebaran.IItemsMasterService _itemsMasterService;
        private readonly Domain.Aldebaran.IRecoveryDomainService _recoveryDomainService;
        private readonly Core.Configuration.IConfigurator _configurator;
        private readonly ISyncOrchestrator _syncOrchestrator;
        private readonly Core.IAutomataState _automataState;
        private readonly INotificationService _notificationService;
        private readonly Domain.Aldebaran.IInventoryAutomationConnectionService _inventoryConnectionService;
        private readonly Core.Configuration.IAldebaranApplicationEnvironment _aldebaranEnvironment;
        private readonly Logger _logger;

        public RecoveryService(Domain.Aldebaran.IItemsMasterService itemsMasterService,
            Domain.Aldebaran.IRecoveryDomainService recoveryDomainService,
            ISyncOrchestrator syncOrchestrator,
            Core.Configuration.IConfigurator configurator,
            Core.IAutomataState automataState,
            INotificationService notificationService,
            Domain.Aldebaran.IInventoryAutomationConnectionService inventoryConnectionService,
            Core.Configuration.IAldebaranApplicationEnvironment aldebaranEnvironment)
        {
            _itemsMasterService = itemsMasterService;
            _recoveryDomainService = recoveryDomainService;
            _syncOrchestrator = syncOrchestrator;
            _configurator = configurator;
            _automataState = automataState;
            _inventoryConnectionService = inventoryConnectionService;
            _aldebaranEnvironment = aldebaranEnvironment;
            _notificationService = notificationService;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public bool TryRecoverOnce()
        {
            try
            {
                _logger.Info("RecoveryService: starting single recovery attempt");

                // Only attempt recovery when connectivity is marked as DOWN. If not, skip.
                if (!_automataState.IsDestinationConnectivityDown)
                {
                    _logger.Info("RecoveryService: connectivity is not marked DOWN. Skipping recovery.");
                    return false;
                }

                // Confirm connectivity restored for origin + destinations before starting recovery
                if (!ConfirmConnectivityRestored(out var failedList))
                {
                    _logger.Warn($"RecoveryService: connectivity check failed. {failedList.Count} failed connections. Aborting recovery.");
                    return false;
                }

                // New orchestration: per-item recovery using master items and domain recovery service
                var syncAttempts = 0;
                int.TryParse(_configurator.GetKey("SyncAttempts"), out syncAttempts);
                if (syncAttempts <= 0) syncAttempts = 1;

                var batchSize = 0;
                int.TryParse(_configurator.GetKey("Recovery.BatchSize"), out batchSize);
                if (batchSize <= 0) batchSize = 10;

                // Global mode is always enabled for recovery (massive two-phase flow over candidates)
                var ranGlobal = false;

                var flagAttempts = int.MaxValue - 1000;
                var itemTimeoutSeconds = 0;
                int.TryParse(_configurator.GetKey("Recovery.ItemTimeoutSeconds"), out itemTimeoutSeconds);
                if (itemTimeoutSeconds <= 0) itemTimeoutSeconds = 300;

                var candidates = _recoveryDomainService.GetCandidateItemIds(syncAttempts).ToList();
                if (!candidates.Any())
                {
                    _logger.Info("RecoveryService: no eligible articles to recover");
                    return false;
                }
                var toProcess = candidates.Take(batchSize).ToList();
                var processedArticles = 0;
                // Always run global two-phase flow over the candidate set
                _logger.Info("RecoveryService: running global two-phase recovery on candidate set");
                try
                {
                    var globalBatchSize = 0;
                    int.TryParse(_configurator.GetKey("Recovery.GlobalBatchSize"), out globalBatchSize);
                    if (globalBatchSize <= 0) globalBatchSize = 20; // default: 20 items per batch

                    // process candidates in batches to avoid huge enqueues
                    for (int offset = 0; offset < candidates.Count; offset += globalBatchSize)
                    {
                        var batch = candidates.Skip(offset).Take(globalBatchSize).ToList();

                        _logger.Info($"RecoveryService: global batch processing ids [{offset}..{offset + batch.Count - 1}]");

                        // 1) Mark events for this batch
                        foreach (var id in batch)
                        {
                            _recoveryDomainService.MarkEventsAsFlagged(id, int.MaxValue - 1000);
                        }

                        // 2) Unpublish batch (row-by-row)
                        foreach (var id in batch)
                        {
                            try { _itemsMasterService.UpdateVisibility(id, false); }
                            catch (Exception ex) { _logger.Error($"RecoveryService: error unpublishing ItemId={id} in global batch: {ex}"); }
                        }

                        // 3) Run full sync to process deletions
                        ExecuteSyncOnce(syncAttempts);

                        // 4) Wait until pending events for this batch are processed or timeout
                        var waited = 0;
                        var itemTimeoutSecondsGlobal = 0;
                        int.TryParse(_configurator.GetKey("Recovery.ItemTimeoutSeconds"), out itemTimeoutSecondsGlobal);
                        if (itemTimeoutSecondsGlobal <= 0) itemTimeoutSecondsGlobal = 300;
                        while (batch.Sum(id => _recoveryDomainService.CountPendingEvents(id, syncAttempts)) > 0 && waited < itemTimeoutSecondsGlobal)
                        {
                            System.Threading.Thread.Sleep(1000);
                            waited++;
                        }

                        if (waited >= itemTimeoutSecondsGlobal)
                        {
                            _logger.Warn("RecoveryService: timeout waiting after global unpublish for batch");
                            return false;
                        }

                        // 5) Republish batch
                        foreach (var id in batch)
                        {
                            try { _itemsMasterService.UpdateVisibility(id, true); }
                            catch (Exception ex) { _logger.Error($"RecoveryService: error republishing ItemId={id} in global batch: {ex}"); }
                        }

                        // 6) Run full sync to process insertions
                        ExecuteSyncOnce(syncAttempts);

                        // 7) Wait until pending events for this batch are processed or timeout
                        waited = 0;
                        while (batch.Sum(id => _recoveryDomainService.CountPendingEvents(id, syncAttempts)) > 0 && waited < itemTimeoutSecondsGlobal)
                        {
                            System.Threading.Thread.Sleep(1000);
                            waited++;
                        }

                        if (waited >= itemTimeoutSecondsGlobal)
                        {
                            _logger.Warn("RecoveryService: timeout waiting after global publish for batch");
                            return false;
                        }

                        // 8) Clear events for batch: only remove previously-flagged events
                        foreach (var id in batch)
                        {
                            _recoveryDomainService.ClearEventsForItem(id, flagAttempts);
                        }

                        // After clearing, detect items that still have pending events (failed to reprocess)
                        var failed = batch.Where(id => _recoveryDomainService.CountPendingEvents(id, syncAttempts) > 0).ToList();
                        if (failed.Any())
                        {
                            foreach (var fid in failed)
                            {
                                // Quarantine: mark events as flagged to avoid repeated automatic retries
                                _recoveryDomainService.MarkEventsAsFlagged(fid, flagAttempts);
                                _logger.Warn($"RecoveryService: item {fid} still has pending events after recovery; marked as flagged for manual investigation");
                            }
                            // TODO: notify via _notificationService (not implemented yet)
                        }

                        processedArticles += batch.Count - failed.Count;
                    }

                    _logger.Info("RecoveryService: global recovery completed for candidate set (batched)");
                    ranGlobal = true;
                }
                catch (Exception ex)
                {
                    _logger.Error($"RecoveryService: global recovery failed: {ex}");
                    return false;
                }

                // If global flow already ran, skip per-item processing
                foreach (var artId in toProcess)
                {
                    if (ranGlobal) break;
                    try
                    {
                        _logger.Info($"RecoveryService: starting recovery for ItemId={artId}");

                        // 1) Mark existing integration events so normal sync won't pick them
                        _recoveryDomainService.MarkEventsAsFlagged(artId, flagAttempts);

                        // 2) Unpublish via master items table (row-by-row update)
                        _itemsMasterService.UpdateVisibility(artId, false);

                        // 3) Execute sync runner to process the events generated by unpublish
                        try
                        {
                            ExecuteSyncOnce(syncAttempts);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"RecoveryService: sync execution failed after unpublish for ItemId={artId}: {ex}");
                        }

                        // wait until events generated by unpublish are processed (poll)
                        var waited = 0;
                        while (_recoveryDomainService.CountPendingEvents(artId, syncAttempts) > 0 && waited < itemTimeoutSeconds)
                        {
                            System.Threading.Thread.Sleep(1000);
                            waited++;
                        }

                        if (waited >= itemTimeoutSeconds)
                        {
                            _logger.Warn($"RecoveryService: timeout waiting after unpublish for ItemId={artId}");
                            continue; // skip to next article
                        }

                        // 4) Republish
                        _itemsMasterService.UpdateVisibility(artId, true);

                        // 5) Execute sync runner again to process publish events
                        try
                        {
                            ExecuteSyncOnce(syncAttempts);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"RecoveryService: sync execution failed after publish for ItemId={artId}: {ex}");
                        }

                        // wait until events generated by publish are processed (poll)
                        waited = 0;
                        while (_recoveryDomainService.CountPendingEvents(artId, syncAttempts) > 0 && waited < itemTimeoutSeconds)
                        {
                            System.Threading.Thread.Sleep(1000);
                            waited++;
                        }

                        if (waited >= itemTimeoutSeconds)
                        {
                            _logger.Warn($"RecoveryService: timeout waiting after publish for ItemId={artId}");
                            continue; // skip to next article
                        }

                        // 6) Clear all old events for the item (only previously-flagged)
                        _recoveryDomainService.ClearEventsForItem(artId, flagAttempts);
                        processedArticles++;
                        _logger.Info($"RecoveryService: finished recovery for ItemId={artId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"RecoveryService: error recovering ItemId={artId} | {ex}");
                    }
                }

                if (processedArticles > 0)
                {
                    _logger.Info($"RecoveryService: recovery succeeded for {processedArticles} articles");
                    var since = _automataState.DestinationConnectivityDownSince ?? DateTime.UtcNow;
                    var until = DateTime.UtcNow;
                    _logger.Info("RecoveryService: resetting connectivity error counts and clearing DestinationConnectivityDown flag");
                    _automataState.ResetConnectivityErrorCounts();
                    _automataState.IsDestinationConnectivityDown = false;
                    _automataState.DestinationConnectivityDownSince = null;
                    _logger.Info($"DestinationConnectivityDown cleared. Sync will be reactivated. Articles processed: {processedArticles}");
                    _notificationService.NotifyConnectivityRecovered(processedArticles, since, until);
                    return true;
                }

                _logger.Warn("RecoveryService: no articles were fully recovered in this run");
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error($"RecoveryService error: {ex}");
                return false;
            }
        }

        private void ExecuteSyncOnce(int syncAttempts)
        {
            // Reuse SyncJob.RunOnceForced by resolving it from container and invoking the method
            // execute orchestrator via injected service
            _syncOrchestrator.RunOnce(true);
        }

        // Active connectivity check: verify origin and all destinations are reachable.
        // Returns true if number of failed connections <= allowedFailures (configurable, default 0).
        private bool ConfirmConnectivityRestored(out System.Collections.Generic.List<string> failedConnections)
        {
            failedConnections = new System.Collections.Generic.List<string>();
            try
            {
                var timeoutSeconds = 5;
                int.TryParse(_configurator.GetKey("Recovery.ConnectivityCheckTimeoutSeconds"), out timeoutSeconds);
                if (timeoutSeconds <= 0) timeoutSeconds = 5;

                var allowedFailures = 0;
                int.TryParse(_configurator.GetKey("Recovery.AllowFailedConnections"), out allowedFailures);
                if (allowedFailures < 0) allowedFailures = 0;

                // Check origin (Aldebaran)
                try
                {
                    var connStr = _aldebaranEnvironment.GetConnectionString();
                    using (var conn = new SqlConnection(connStr))
                    {
                        conn.ConnectionString = connStr;
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT 1";
                            cmd.CommandTimeout = timeoutSeconds;
                            cmd.ExecuteScalar();
                        }
                    }
                }
                catch (Exception ex)
                {
                    failedConnections.Add($"ORIGIN: {ex.Message}");
                }

                // Check destinations
                var destinations = _inventoryConnectionService.GetActive().ToList();
                foreach (var dest in destinations)
                {
                    try
                    {
                        var cs = Domain.Aldebaran.InventoryAutomationConnectionStringBuilder.Build(dest);
                        using (var conn = new SqlConnection(cs))
                        {
                            conn.Open();
                            using (var cmd = conn.CreateCommand())
                            {
                                cmd.CommandText = "SELECT 1";
                                cmd.CommandTimeout = timeoutSeconds;
                                cmd.ExecuteScalar();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        failedConnections.Add($"DEST:{dest.InventoryAutomationConnectionId}:{dest.ServerName}:{dest.DatabaseName} => {ex.Message}");
                    }
                }

                return failedConnections.Count <= allowedFailures;
            }
            catch (Exception ex)
            {
                _logger.Error($"ConfirmConnectivityRestored fatal error: {ex}");
                return false;
            }
        }
    }
}
