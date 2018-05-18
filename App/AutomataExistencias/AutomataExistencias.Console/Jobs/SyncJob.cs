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
        private readonly IItemByColorSynchronize _itemByColorSynchronize;
        private readonly IItemSynchronize _itemSynchronize;
        private readonly ILineSynchronize _lineSynchronize;
        private readonly IMoneySynchronize _moneySynchronize;
        private readonly IPackagingSynchronize _packagingSynchronize;
        private readonly IStockSynchronize _stockSynchronize;
        private readonly ITransitOrderSynchronize _transitOrderSynchronize;
        private readonly IUnitMeasuredSynchronize _unitMeasuredSynchronize;
        private readonly IUpdateProcessSynchronize _updateProcessSynchronize;

        private readonly Logger _logger;
        public SyncJob()
        {
            var container = AutofacConfigurator.GetContainer();
            _itemByColorSynchronize = container.Resolve<IItemByColorSynchronize>();
            _itemSynchronize = container.Resolve<IItemSynchronize>();
            _lineSynchronize = container.Resolve<ILineSynchronize>();
            _moneySynchronize = container.Resolve<IMoneySynchronize>();
            _packagingSynchronize = container.Resolve<IPackagingSynchronize>();
            _transitOrderSynchronize = container.Resolve<ITransitOrderSynchronize>();
            _unitMeasuredSynchronize = container.Resolve<IUnitMeasuredSynchronize>();
            _updateProcessSynchronize = container.Resolve<IUpdateProcessSynchronize>();
            _stockSynchronize = container.Resolve<IStockSynchronize>();
            _logger = LogManager.GetCurrentClassLogger();
        }
        private void Sync(string key)
        {
            switch (key)
            {
                case "MoneyJob":
                    _moneySynchronize.Sync();
                    break;
                case "MoneyReverseJob":
                    _moneySynchronize.ReverseSync();
                    break;
                case "UnitMeasuredJob":
                    _unitMeasuredSynchronize.Sync();
                    break;
                case "UnitMeasuredReverseJob":
                    _unitMeasuredSynchronize.ReverseSync();
                    break;
                case "LinesJob":
                    _lineSynchronize.Sync();
                    break;
                case "LinesReverseJob":
                    _lineSynchronize.ReverseSync();
                    break;
                case "ItemsJob":
                    _itemSynchronize.Sync();
                    break;
                case "ItemsReverseJob":
                    _itemSynchronize.ReverseSync();
                    break;
                case "ItemsByColorJob":
                    _itemByColorSynchronize.Sync();
                    break;
                case "ItemsByColorReverseJob":
                    _itemByColorSynchronize.ReverseSync();
                    break;
                case "TransitOrderJob":
                    _transitOrderSynchronize.Sync();
                    break;
                case "TransitOrderReverseJob":
                    _transitOrderSynchronize.ReverseSync();
                    break;
                case "StockJob":
                    _stockSynchronize.Sync();
                    break;
                case "StockReverseJob":
                    _stockSynchronize.ReverseSync();
                    break;
                case "PackagingJob":
                    _packagingSynchronize.Sync();
                    break;
                case "PackagingReverseJob":
                    _packagingSynchronize.ReverseSync();
                    break;
                case "UpdateProcessJob":
                    _updateProcessSynchronize.UpdateProcess();
                    break;                
                default:
                    _logger.Warn($"Key [{key}] not identified");
                    break;
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            var scheduleSequence = System.Configuration.ConfigurationManager.AppSettings["Schedule.Sequence"].Split(';').ToList();
            var scheduleReverseSequence = System.Configuration.ConfigurationManager.AppSettings["Schedule.Sequence.Reverse"].Split(';').ToList();

            var schedule = scheduleSequence;
            schedule.AddRange(scheduleReverseSequence);

            foreach (var jobKey in schedule)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                _logger.Info($"[{jobKey}] has started");
                try
                {
                    Sync(jobKey);
                }
                catch (Exception ex)
                {
                    _logger.Error($"An exception has occurred while execution of {jobKey} | Exception: {ex.ToJson()}");
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
