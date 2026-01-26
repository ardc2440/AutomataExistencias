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
        private readonly Domain.Aldebaran.IItemService _itemService;
        private readonly INotificationService _notificationService;
        private readonly IConnectivityErrorClassifier _connectivityErrorClassifier;
        private readonly IConfigurator _configurator;
        private readonly Logger _logger;

        public NonConnectivityErrorsJob()
        {
            var container = AutofacConfigurator.GetContainer();
            _itemService = container.Resolve<Domain.Aldebaran.IItemService>();
            _notificationService = container.Resolve<INotificationService>();
            _connectivityErrorClassifier = container.Resolve<IConnectivityErrorClassifier>();
            _configurator = container.Resolve<IConfigurator>();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Execute(IJobExecutionContext context)
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

                var candidates = _itemService.Get().Where(i => i.Attempts >= syncAttempts).ToList();
                var nonConnectivity = candidates.Where(i =>
                {
                    try
                    {
                        return !_connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception);
                    }
                    catch
                    {
                        return true;
                    }
                }).ToList();

                if (nonConnectivity.Any())
                {
                    _logger.Info($"[NonConnectivityErrorsJob] Found {nonConnectivity.Count} non-connectivity items with attempts >= {syncAttempts}");
                    _notificationService.NotifyNonConnectivityErrors(nonConnectivity, since);
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
