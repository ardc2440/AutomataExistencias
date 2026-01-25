using System;
using System.Collections.Generic;
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
    public class SyncJob : IJob
    {
        private readonly AutomataExistencias.Core.IAutomataState _automataState;
        private readonly AutomataExistencias.Application.IConnectivityErrorClassifier _connectivityErrorClassifier;
        private readonly AutomataExistencias.Application.INotificationService _notificationService;
        private readonly Domain.Aldebaran.IAutomataConnectivityThresholdService _thresholdService;
        private readonly Domain.Aldebaran.IInventoryAutomationConnectionService _inventoryConnectionService;
        /*ItemByColor*/
        private readonly IItemByColorSynchronize _itemByColorSynchronize;
        private readonly Domain.Aldebaran.IItemByColorService _aldebaranItemByColorService;
        /*Item*/
        private readonly IItemSynchronize _itemSynchronize;
        private readonly Domain.Aldebaran.IItemService _aldebaranItemService;
        /*Lines*/
        private readonly ILineSynchronize _lineSynchronize;
        private readonly Domain.Aldebaran.ILineService _aldebaranLineService;
        /*Money*/
        private readonly IMoneySynchronize _moneySynchronize;
        private readonly Domain.Aldebaran.IMoneyService _aldebaranMoneyService;
        /*Packaging*/
        private readonly IPackagingSynchronize _packagingSynchronize;
        private readonly Domain.Aldebaran.IPackagingService _aldebaranPackagingService;
        /*Stock*/
        private readonly IStockSynchronize _stockSynchronize;
        private readonly Domain.Aldebaran.IStockService _aldebaranStockService;
        /*TransitOrder*/
        private readonly ITransitOrderSynchronize _transitOrderSynchronize;
        private readonly Domain.Aldebaran.ITransitOrderService _aldebaranTransitOrderService;
        /*UnitMeasured*/
        private readonly IUnitMeasuredSynchronize _unitMeasuredSynchronize;
        private readonly Domain.Aldebaran.IUnitMeasuredService _aldebaranUnitMeasuredService;
        /*UpdateProcess*/
        private readonly IUpdateProcessSynchronize _updateProcessSynchronize;        
        /*Others*/
        private readonly Logger _logger;
        private readonly IConfigurator _configurator;
        private readonly int _syncAttempts;
        private readonly int _windowMinutes;
        private readonly double _percentThreshold;
        private readonly int _minAttempts;
        private readonly int _consecutiveThreshold;
        /*Data*/
        private IEnumerable<DataAccess.Aldebaran.Line> _lineData;
        private IEnumerable<DataAccess.Aldebaran.Money> _moneyData;
        private IEnumerable<DataAccess.Aldebaran.UnitMeasured> _unitMeasuredData;
        private IEnumerable<DataAccess.Aldebaran.Item> _itemData;
        private IEnumerable<DataAccess.Aldebaran.ItemByColor> _itembyColorData;
        private IEnumerable<DataAccess.Aldebaran.TransitOrder> _transitOrderData;
        private IEnumerable<DataAccess.Aldebaran.Stock> _stockData;
        private IEnumerable<DataAccess.Aldebaran.Packaging> _packagingData;

        public SyncJob()
        {
            var container = AutofacConfigurator.GetContainer();          
            /*ItemByColor*/
            _itemByColorSynchronize = container.Resolve<IItemByColorSynchronize>();
            _aldebaranItemByColorService = container.Resolve<Domain.Aldebaran.IItemByColorService>();
            /*Item*/
            _itemSynchronize = container.Resolve<IItemSynchronize>();
            _aldebaranItemService = container.Resolve<Domain.Aldebaran.IItemService>();
            /*Lines*/
            _lineSynchronize = container.Resolve<ILineSynchronize>();
            _aldebaranLineService = container.Resolve<Domain.Aldebaran.ILineService>();
            /*Money*/
            _moneySynchronize = container.Resolve<IMoneySynchronize>();
            _aldebaranMoneyService = container.Resolve<Domain.Aldebaran.IMoneyService>();
            /*Packaging*/
            _packagingSynchronize = container.Resolve<IPackagingSynchronize>();
            _aldebaranPackagingService = container.Resolve<Domain.Aldebaran.IPackagingService>();
            /*Stock*/
            _stockSynchronize = container.Resolve<IStockSynchronize>();
            _aldebaranStockService = container.Resolve<Domain.Aldebaran.IStockService>();
            /*TransitOrder*/
            _transitOrderSynchronize = container.Resolve<ITransitOrderSynchronize>();
            _aldebaranTransitOrderService = container.Resolve<Domain.Aldebaran.ITransitOrderService>();
            /*UnitMeasured*/
            _unitMeasuredSynchronize = container.Resolve<IUnitMeasuredSynchronize>();
            _aldebaranUnitMeasuredService = container.Resolve<Domain.Aldebaran.IUnitMeasuredService>();
            /*UpdateProcess*/ 
            _updateProcessSynchronize = container.Resolve<IUpdateProcessSynchronize>();
            _automataState = container.Resolve<AutomataExistencias.Core.IAutomataState>();
            _connectivityErrorClassifier = container.Resolve<AutomataExistencias.Application.IConnectivityErrorClassifier>();
            _notificationService = container.Resolve<AutomataExistencias.Application.INotificationService>();
            _thresholdService = container.Resolve<Domain.Aldebaran.IAutomataConnectivityThresholdService>();
            _inventoryConnectionService = container.Resolve<Domain.Aldebaran.IInventoryAutomationConnectionService>();
            /*Others*/
            var configurator = container.Resolve<IConfigurator>();
            _configurator = configurator;
            _syncAttempts = configurator.GetKey("SyncAttempts").ToInt();

            // Connectivity detection settings from config (safe parse)
            int.TryParse(configurator.GetKey("ConnectivityError.WindowMinutes"), out _windowMinutes);
            if (_windowMinutes <= 0) _windowMinutes = 15;
            double.TryParse(configurator.GetKey("ConnectivityError.PercentThreshold"), out _percentThreshold);
            if (_percentThreshold <= 0) _percentThreshold = 70;
            int.TryParse(configurator.GetKey("ConnectivityError.MinAttempts"), out _minAttempts);
            if (_minAttempts <= 0) _minAttempts = 5;
            int.TryParse(configurator.GetKey("ConnectivityError.ConsecutiveThreshold"), out _consecutiveThreshold);
            if (_consecutiveThreshold <= 0) _consecutiveThreshold = 10;
            _logger = LogManager.GetCurrentClassLogger();
        }

        // Run full sync sequence regardless of AutomataState (used by Recovery)
        public void RunOnceForced()
        {
            var scheduleSequence = System.Configuration.ConfigurationManager.AppSettings["Schedule.Sequence"].Split(';').ToList();
            var scheduleReverseSequence = System.Configuration.ConfigurationManager.AppSettings["Schedule.Sequence.Reverse"].Split(';').ToList();

            var schedule = scheduleSequence;
            schedule.AddRange(scheduleReverseSequence);

            _lineData = _aldebaranLineService.Get(_syncAttempts);
            _moneyData = _aldebaranMoneyService.Get(_syncAttempts);
            _unitMeasuredData = _aldebaranUnitMeasuredService.Get(_syncAttempts);
            _itemData = _aldebaranItemService.Get(_syncAttempts);
            _itembyColorData = _aldebaranItemByColorService.Get(_syncAttempts);
            _transitOrderData = _aldebaranTransitOrderService.Get(_syncAttempts);
            _stockData = _aldebaranStockService.Get(_syncAttempts);
            _packagingData = _aldebaranPackagingService.Get(_syncAttempts);

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
                    _logger.Error($"An exception has occurred while execution of {jobKey} | Exception: {ex}");
                    try
                    {
                        if (_connectivityErrorClassifier.IsDestinationConnectivityError(ex.Message))
                        {
                            _automataState.RecordAttempt(0, true);
                        }
                    }
                    catch { }
                }
                finally
                {
                    watch.Stop();
                    var elapsedMs = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
                    _logger.Info($"[{jobKey}] has finished in {elapsedMs.ToReadableString()}");
                }
            }

            // After running all jobs, evaluate connectivity error counts to decide if we should mark DOWN
            try
            {
                var totalAttempts = _automataState.GetTotalAttempts(_windowMinutes);
                var totalErrors = _automataState.GetConnectivityErrorCount(_windowMinutes);
                var pct = _automataState.GetConnectivityErrorPercentage(_windowMinutes);

                _logger.Info($"Connectivity window {_windowMinutes}min: attempts={totalAttempts}, errors={totalErrors}, percent={pct:0.##}%");

                if (totalAttempts >= _minAttempts && pct >= _percentThreshold)
                {
                    if (!_automataState.IsDestinationConnectivityDown)
                    {
                        _automataState.IsDestinationConnectivityDown = true;
                        if (_automataState.DestinationConnectivityDownSince == null)
                            _automataState.DestinationConnectivityDownSince = DateTime.UtcNow;

                        var connections = _inventoryConnectionService.GetActive().Where(c => _automataState.GetConnectionsWithErrors().Contains(c.InventoryAutomationConnectionId)).ToList();
                        _notificationService.NotifyConnectivityDown(connections, _automataState.DestinationConnectivityDownSince.Value);
                    }
                }

                // Also check consecutive failures per connection
                var activeConnections = _inventoryConnectionService.GetActive();
                foreach (var conn in activeConnections)
                {
                    var cons = _automataState.GetConsecutiveFailures(conn.InventoryAutomationConnectionId);
                    if (cons >= _consecutiveThreshold)
                    {
                        if (!_automataState.IsDestinationConnectivityDown)
                        {
                            _automataState.IsDestinationConnectivityDown = true;
                            if (_automataState.DestinationConnectivityDownSince == null)
                                _automataState.DestinationConnectivityDownSince = DateTime.UtcNow;

                            _notificationService.NotifyConnectivityDown(new[] { conn }, _automataState.DestinationConnectivityDownSince.Value);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error evaluating connectivity error thresholds: {ex}");
            }
        }
        private void Sync(string key)
        {
            
            switch (key)
            {
                case "MoneyJob":
                    _moneySynchronize.Sync(_moneyData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "MoneyReverseJob":
                    _moneySynchronize.ReverseSync(_moneyData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "UnitMeasuredJob":
                    _unitMeasuredSynchronize.Sync(_unitMeasuredData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "UnitMeasuredReverseJob":
                    _unitMeasuredSynchronize.ReverseSync(_unitMeasuredData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "LinesJob":
                    _lineSynchronize.Sync(_lineData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "LinesReverseJob":
                    _lineSynchronize.ReverseSync(_lineData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "ItemsJob":
                    _itemSynchronize.Sync(_itemData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "ItemsReverseJob":
                    _itemSynchronize.ReverseSync(_itemData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "ItemsByColorJob":
                    _itemByColorSynchronize.Sync(_itembyColorData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "ItemsByColorReverseJob":
                    _itemByColorSynchronize.ReverseSync(_itembyColorData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "TransitOrderJob":
                    _transitOrderSynchronize.Sync(_transitOrderData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "TransitOrderReverseJob":
                    _transitOrderSynchronize.ReverseSync(_transitOrderData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "StockJob":
                    _stockSynchronize.Sync(_stockData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "StockReverseJob":
                    _stockSynchronize.ReverseSync(_stockData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "PackagingJob":
                    _packagingSynchronize.Sync(_packagingData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
                    break;
                case "PackagingReverseJob":
                    _packagingSynchronize.ReverseSync(_packagingData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), _syncAttempts);
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
            if (_automataState.IsDestinationConnectivityDown || _automataState.IsOriginConnectivityDown)
            {
                _logger.Warn("Destination or origin connectivity is marked as DOWN. Skipping SyncJob execution.");
                return;
            }

            // Per-run we do not fully reset sliding-window counters; keep window behavior
            // but we may clear short-lived per-run structures if needed. For now keep as-is.

            // Execute the normal sync sequence
            RunOnceForced();

            // After running all jobs, evaluate connectivity error counts to decide if we should mark DOWN
            try
            {
                var totalAttempts = _automataState.GetTotalAttempts(_windowMinutes);
                var totalErrors = _automataState.GetConnectivityErrorCount(_windowMinutes);
                var pct = _automataState.GetConnectivityErrorPercentage(_windowMinutes);

                _logger.Info($"Connectivity window {_windowMinutes}min: attempts={totalAttempts}, errors={totalErrors}, percent={pct:0.##}%");

                if (totalAttempts >= _minAttempts && pct >= _percentThreshold)
                {
                    if (!_automataState.IsDestinationConnectivityDown)
                    {
                        _automataState.IsDestinationConnectivityDown = true;
                        if (_automataState.DestinationConnectivityDownSince == null)
                            _automataState.DestinationConnectivityDownSince = DateTime.UtcNow;

                        var connections = _inventoryConnectionService.GetActive().Where(c => _automataState.GetConnectionsWithErrors().Contains(c.InventoryAutomationConnectionId)).ToList();
                        _notificationService.NotifyConnectivityDown(connections, _automataState.DestinationConnectivityDownSince.Value);
                    }
                }

                // Also check consecutive failures per connection
                var activeConnections = _inventoryConnectionService.GetActive();
                foreach (var conn in activeConnections)
                {
                    var cons = _automataState.GetConsecutiveFailures(conn.InventoryAutomationConnectionId);
                    if (cons >= _consecutiveThreshold)
                    {
                        if (!_automataState.IsDestinationConnectivityDown)
                        {
                            _automataState.IsDestinationConnectivityDown = true;
                            if (_automataState.DestinationConnectivityDownSince == null)
                                _automataState.DestinationConnectivityDownSince = DateTime.UtcNow;

                            _notificationService.NotifyConnectivityDown(new[] { conn }, _automataState.DestinationConnectivityDownSince.Value);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error evaluating connectivity error thresholds: {ex.ToJson()}");
            }
        }
    }
}
