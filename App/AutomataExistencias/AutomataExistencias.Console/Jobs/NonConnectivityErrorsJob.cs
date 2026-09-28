using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Console.Code;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using NLog;
using Quartz;

namespace AutomataExistencias.Console.Jobs
{
    [DisallowConcurrentExecution]
    public class NonConnectivityErrorsJob : IJob
    {
        private Domain.Aldebaran.IItemService _itemService;
        private INotificationService _notificationService;
        private IConnectivityErrorClassifier _connectivityErrorClassifier;
        private IConfigurator _configurator;
        private Logger _logger;

        private void ResolveDependencies(ILifetimeScope container)
        {
            _itemService = container.Resolve<Domain.Aldebaran.IItemService>();
            _notificationService = container.Resolve<INotificationService>();
            _connectivityErrorClassifier = container.Resolve<IConnectivityErrorClassifier>();
            _configurator = container.Resolve<IConfigurator>();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Execute(IJobExecutionContext context)
        {
            // Hotfix_CaidaServicio: cada ejecucion usa su propio lifetime scope de Autofac.
            // Al salir del using se liberan (Dispose) los DbContext creados en la ejecucion;
            // antes se resolvian desde el contenedor raiz y quedaban retenidos (fuga de memoria).
            try
            {
                using (var scope = AutofacConfigurator.GetContainer().BeginLifetimeScope())
                {
                    ResolveDependencies(scope);
                    ExecuteInternal();
                }
            }
            catch (Exception ex)
            {
                // No propagar a Quartz: la siguiente ejecucion debe dispararse normalmente.
                LogManager.GetCurrentClassLogger().Error($"[NonConnectivityErrorsJob] unhandled error: {ex}");
            }
        }

        private void ExecuteInternal()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Info("[NonConnectivityErrorsJob] has started");
            try
            {
                var syncAttempts = _configurator.GetKey("SyncAttempts").ToInt();

                // Interval used for scheduling/notification "since" calculation
                var intervalStr = _configurator.GetKey("Notification.NonConnectivityInterval");
                TimeSpan interval;
                if (!TimeSpan.TryParse(intervalStr, out interval))
                    interval = TimeSpan.FromMinutes(30);

                var since = DateTime.UtcNow.Subtract(interval);

                // Hotfix_CaidaServicio: filtro en SQL; antes Get() traia toda la tabla RITEMS a memoria (OOM 27-sep).
                var candidates = _itemService.GetWithAttempts().ToList();

                // Use connectivity window minutes to decide whether a recent connectivity error
                // should be treated as pending connectivity or classified as business (older than window)
                int windowMinutes;
                if (!int.TryParse(_configurator.GetKey("ConnectivityError.WindowMinutes"), out windowMinutes) || windowMinutes <= 0)
                    windowMinutes = 15;

                var nonConnectivity = new System.Collections.Generic.List<string>();
                var pendingConnectivity = new System.Collections.Generic.List<string>();

                foreach (var i in candidates)
                {
                    try
                    {
                        var isConn = false;
                        try { isConn = _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception); } catch { isConn = false; }

                        // Attempt to get date (FECHA_INTEGRA or Date or Fecha). If not available, treat as old.
                        DateTime? dt = null;
                        try
                        {
                            var prop = i.GetType().GetProperty("FECHA_INTEGRA") ?? i.GetType().GetProperty("Date") ?? i.GetType().GetProperty("Fecha");
                            if (prop != null)
                            {
                                var v = prop.GetValue(i);
                                if (v is DateTime d) dt = d.ToUniversalTime();
                                else if (DateTime.TryParse(v?.ToString(), out var pd)) dt = pd.ToUniversalTime();
                            }
                        }
                        catch { dt = null; }

                        var now = DateTime.UtcNow;
                        var safeException = (i.Exception ?? string.Empty).Replace('\r', ' ').Replace('\n', ' ');
                        // Hotfix_CaidaServicio: limitar tamano del texto que va al correo.
                        if (safeException.Length > 1000) safeException = safeException.Substring(0, 1000) + "...";
                        var desc = $"Id={i.Id} | Attempts={i.Attempts} | Err={safeException}";

                        if (!isConn)
                        {
                            nonConnectivity.Add(desc);
                        }
                        else
                        {
                            // If FECHA_INTEGRA older than connectivity window => treat as business (nonConnectivity)
                            bool olderThanWindow = dt.HasValue && (now - dt.Value).TotalMinutes > windowMinutes;
                            // If no date available, treat as old (safer path)
                            bool olderThan10Min = olderThanWindow || !dt.HasValue;
                            if (olderThan10Min)
                            {
                                nonConnectivity.Add(desc);
                            }
                            else
                            {
                                // Recent (<=10 min) -> pendingConnectivity if attempts >= syncAttempts or older than recovery timeout
                                if (i.Attempts >= syncAttempts)
                                {
                                    pendingConnectivity.Add(desc);
                                }
                                else
                                {
                                    var recoveryTimeout = _configurator.GetKey("Recovery.ItemTimeoutSeconds").ToInt();
                                    bool olderThanRecovery = dt.HasValue && (now - dt.Value).TotalSeconds > recoveryTimeout;
                                    if (olderThanRecovery)
                                        pendingConnectivity.Add(desc);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn($"Error processing item in NonConnectivityErrorsJob: {ex}");
                    }
                }

                if (nonConnectivity.Any() || pendingConnectivity.Any())
                {
                    _logger.Info($"[NonConnectivityErrorsJob] Found nonConnectivity={nonConnectivity.Count} pendingConnectivity={pendingConnectivity.Count}");
                    _notificationService.NotifyPendingAndNonConnectivityErrors(nonConnectivity, pendingConnectivity, since);
                }
                else
                {
                    _logger.Info("[NonConnectivityErrorsJob] No non-connectivity items to notify");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[NonConnectivityErrorsJob] failed: {ex}");
            }
            finally
            {
                watch.Stop();
                var elapsedMs = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
                _logger.Info($"[NonConnectivityErrorsJob] has finished in {elapsedMs}");
            }
        }
    }
}
