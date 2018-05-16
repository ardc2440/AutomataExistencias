using System;
using System.Linq;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Console.Code;
using AutomataExistencias.Core.Extensions;
using NLog;
using Quartz;

namespace AutomataExistencias.Console.Jobs
{
    [DisallowConcurrentExecution]
    public class SyncJob : IJob
    {
        private readonly ISynchronize _synchronize;
        private readonly Logger _logger;
        public SyncJob()
        {
            var container = AutofacConfigurator.GetContainer();
            _synchronize = container.Resolve<ISynchronize>();
            _logger = LogManager.GetCurrentClassLogger();
        }
        private void Sync(string key)
        {
            switch (key)
            {
                case "MoneyJob":
                    _synchronize.MoneySync();
                    break;
                case "UnitMeasuredJob":
                    _synchronize.UnitMeasuredSync();
                    break;
                case "LinesJob":
                    _synchronize.LinesSync();
                    break;
                case "ItemsJob":
                    _synchronize.ItemsSync();
                    break;
                case "ItemsByColorJob":
                    _synchronize.ItemsByColorSync();
                    break;
                case "TransitOrderJob":
                    _synchronize.TransitOrderSync();
                    break;
                case "StockJob":
                    _synchronize.StockSync();
                    break;
                case "UpdateProcessJob":
                    _synchronize.UpdateProcess();
                    break;
                default:
                    _logger.Warn($"Key [{key}] not identified");
                    break;
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            var scheduleSequence = System.Configuration.ConfigurationManager.AppSettings["Schedule.Sequence"].Split(';').ToList();
            foreach (var jobKey in scheduleSequence)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                _logger.Info($"[{jobKey}] has started");
                try
                {
                    Sync(jobKey);
                }
                catch (Exception ex)
                {
                    _logger.Error($"An exception has occurred while Sync of {jobKey} | Exception: {ex.ToJson()}");
                }
                finally
                {
                    watch.Stop();
                    var elapsedMs = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
                    _logger.Info($"[{jobKey}] has finished in {elapsedMs.ToReadableString()}");
                }
            }

        }
    }
}
