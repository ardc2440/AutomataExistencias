using System;
using System.Linq;
using System.Data.SqlClient;
using NLog;

namespace AutomataExistencias.Application
{
    public class RecoveryService : IRecoveryService
    {
        private readonly Domain.Aldebaran.IItemService _itemService;
        private readonly Domain.Aldebaran.IStockService _stockService;
        private readonly Domain.Aldebaran.IPackagingService _packagingService;
        private readonly Domain.Aldebaran.ITransitOrderService _transitOrderService;
        private readonly Domain.Aldebaran.IItemByColorService _itemByColorService;
        private readonly IItemSynchronize _itemSynchronize;
        private readonly IStockSynchronize _stockSynchronize;
        private readonly IPackagingSynchronize _packagingSynchronize;
        private readonly ITransitOrderSynchronize _transitOrderSynchronize;
        private readonly IItemByColorSynchronize _itemByColorSynchronize;
        private readonly IMoneySynchronize _moneySynchronize;
        private readonly IUnitMeasuredSynchronize _unitMeasuredSynchronize;
        private readonly IConnectivityErrorClassifier _connectivityErrorClassifier;
        private readonly AutomataExistencias.Core.IAutomataState _automataState;
        private readonly AutomataExistencias.Application.INotificationService _notificationService;
        private readonly Domain.Aldebaran.IInventoryAutomationConnectionService _inventoryConnectionService;
        private readonly Domain.Aldebaran.IItemsMasterService _itemsMasterService;
        private readonly Domain.Aldebaran.IRecoveryDomainService _recoveryDomainService;
        private readonly AutomataExistencias.Core.Configuration.IConfigurator _configurator;
        private readonly ISyncOrchestrator _syncOrchestrator;
        private readonly Logger _logger;

        public RecoveryService(Domain.Aldebaran.IItemService itemService,
            Domain.Aldebaran.IStockService stockService,
            Domain.Aldebaran.IPackagingService packagingService,
            Domain.Aldebaran.ITransitOrderService transitOrderService,
            Domain.Aldebaran.IItemByColorService itemByColorService,
            IItemSynchronize itemSynchronize,
            IStockSynchronize stockSynchronize,
            IPackagingSynchronize packagingSynchronize,
            ITransitOrderSynchronize transitOrderSynchronize,
            IItemByColorSynchronize itemByColorSynchronize,
            IMoneySynchronize moneySynchronize,
            IUnitMeasuredSynchronize unitMeasuredSynchronize,
            IConnectivityErrorClassifier connectivityErrorClassifier,
            AutomataExistencias.Core.IAutomataState automataState,
            INotificationService notificationService,
            Domain.Aldebaran.IInventoryAutomationConnectionService inventoryConnectionService,
            Domain.Aldebaran.IItemsMasterService itemsMasterService,
            Domain.Aldebaran.IRecoveryDomainService recoveryDomainService,
            ISyncOrchestrator syncOrchestrator,
            AutomataExistencias.Core.Configuration.IConfigurator configurator)
        {
            _itemService = itemService;
            _stockService = stockService;
            _packagingService = packagingService;
            _transitOrderService = transitOrderService;
            _itemByColorService = itemByColorService;
            _itemSynchronize = itemSynchronize;
            _stockSynchronize = stockSynchronize;
            _packagingSynchronize = packagingSynchronize;
            _transitOrderSynchronize = transitOrderSynchronize;
            _itemByColorSynchronize = itemByColorSynchronize;
            _moneySynchronize = moneySynchronize;
            _unitMeasuredSynchronize = unitMeasuredSynchronize;
            _connectivityErrorClassifier = connectivityErrorClassifier;
            _automataState = automataState;
            _notificationService = notificationService;
            _inventoryConnectionService = inventoryConnectionService;
            _itemsMasterService = itemsMasterService;
            _recoveryDomainService = recoveryDomainService;
            _syncOrchestrator = syncOrchestrator;
            _configurator = configurator;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public bool TryRecoverOnce()
        {
            try
            {
                _logger.Info("RecoveryService: starting single recovery attempt");

                // New orchestration: per-item recovery using master items and domain recovery service
                var syncAttempts = 0;
                int.TryParse(_configurator.GetKey("SyncAttempts"), out syncAttempts);
                if (syncAttempts <= 0) syncAttempts = 1;

                var batchSize = 0;
                int.TryParse(_configurator.GetKey("Recovery.BatchSize"), out batchSize);
                if (batchSize <= 0) batchSize = 10;

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

                foreach (var artId in toProcess)
                {
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

                        // 6) Clear all old events for the item
                        _recoveryDomainService.ClearEventsForItem(artId);
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
                    _automataState.ResetConnectivityErrorCounts();
                    _automataState.IsDestinationConnectivityDown = false;
                    _automataState.DestinationConnectivityDownSince = null;
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
    }
}
