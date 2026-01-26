using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Console.Code;
using NLog;
using Quartz;

namespace AutomataExistencias.Console.Jobs
{
    [DisallowConcurrentExecution]
    public class RecoveryJob : IJob
    {
        private readonly IRecoveryService _recoveryService;
        private readonly Logger _logger;

        public RecoveryJob()
        {
            var container = AutofacConfigurator.GetContainer();
            _recoveryService = container.Resolve<IRecoveryService>();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var container = AutofacConfigurator.GetContainer();
                var automataState = container.Resolve<AutomataExistencias.Core.IAutomataState>();
                if (!automataState.IsDestinationConnectivityDown)
                {
                    _logger.Info("[RecoveryJob] skipping execution because destination connectivity is not marked DOWN.");
                    return;
                }
                // Before running global recovery, validate all active destinations are reachable to avoid re-queuing events
                try
                {
                    var catRunner = container.Resolve<AutomataExistencias.Application.ICatapromDestinationRunner>();
                    var inventoryConnService = container.Resolve<AutomataExistencias.Domain.Aldebaran.IInventoryAutomationConnectionService>();
                    var notificationService = container.Resolve<AutomataExistencias.Application.INotificationService>();

                    var activeConnections = inventoryConnService.GetActive()?.ToList() ?? new System.Collections.Generic.List<AutomataExistencias.DataAccess.Aldebaran.InventoryAutomationConnection>();
                    if (!activeConnections.Any())
                    {
                        _logger.Warn("[RecoveryJob] no active destinations configured. Skipping recovery.");
                        return;
                    }

                    var succeededIds = new System.Collections.Generic.HashSet<int>();
                    int timeoutSeconds = 5;
                    int.TryParse(container.Resolve<AutomataExistencias.Core.Configuration.IConfigurator>().GetKey("Recovery.DestinationCheckTimeoutSeconds"), out timeoutSeconds);
                    if (timeoutSeconds <= 0) timeoutSeconds = 5;

                    // Run a light connectivity check per destination using the existing runner. We consider a destination healthy
                    // only if its per-destination callback completes a trivial DB open within the timeout.
                    try
                    {
                        catRunner.RunForAllDestinations((uow, conn) =>
                        {
                            try
                            {
                                var unit = uow as AutomataExistencias.DataAccess.Core.Contract.IUnitOfWork;
                                if (unit == null)
                                    return;

                                var ctx = unit.GetContext();
                                var dbConn = ctx.Database.Connection;

                                var task = System.Threading.Tasks.Task.Run(() =>
                                {
                                    try
                                    {
                                        if (dbConn.State != System.Data.ConnectionState.Open)
                                            dbConn.Open();
                                        dbConn.Close();
                                    }
                                    catch { throw; }
                                });

                                if (task.Wait(TimeSpan.FromSeconds(timeoutSeconds)))
                                {
                                    lock (succeededIds)
                                        succeededIds.Add(conn.InventoryAutomationConnectionId);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Debug($"[RecoveryJob] destination check failed for ConnId={conn.InventoryAutomationConnectionId}: {ex.Message}");
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn($"[RecoveryJob] error executing destination checks: {ex}");
                    }

                    // Compare active connections vs succeededIds - require ALL active to respond
                    var failed = activeConnections.Where(a => !succeededIds.Contains(a.InventoryAutomationConnectionId)).ToList();
                    if (failed.Any())
                    {
                        _logger.Warn($"[RecoveryJob] some active destinations are unreachable. Skipping recovery. Failed count={failed.Count}");
                        try
                        {
                            var since = container.Resolve<AutomataExistencias.Core.IAutomataState>().DestinationConnectivityDownSince ?? DateTime.UtcNow;
                            notificationService.NotifyConnectivityDown(failed, since);
                        }
                        catch (Exception ex)
                        {
                            _logger.Warn($"[RecoveryJob] could not send notification for unreachable destinations: {ex}");
                        }
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn($"[RecoveryJob] could not perform strict destination connectivity test: {ex}");
                    // proceed with recovery as a fallback
                }
            }
            catch (Exception ex)
            {
                _logger.Warn($"[RecoveryJob] could not resolve AutomataState to check connectivity flag: {ex}");
                // proceed, RecoveryService will also check flag
            }
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Info("[RecoveryJob] has started");
            try
            {
                var ok = _recoveryService.TryRecoverOnce();
                _logger.Info($"[RecoveryJob] finished. Success={ok}");
            }
            catch (Exception ex)
            {
                _logger.Error($"An exception has occurred while execution of RecoveryJob | Exception: {ex}");
            }
            finally
            {
                watch.Stop();
                var elapsedMs = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
                _logger.Info($"[RecoveryJob] has finished in {elapsedMs}");
            }
        }
    }
}
