using System;
using System.Linq;
using NLog;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;

namespace AutomataExistencias.Application
{
    public class SyncOrchestratorService : ISyncOrchestrator
    {
        private readonly IConfigurator _configurator;
        private readonly Logger _logger;
        private readonly AutomataExistencias.Core.IAutomataState _automataState;
        private readonly IConnectivityErrorClassifier _connectivityClassifier;
        private readonly INotificationService _notificationService;
        private readonly Domain.Aldebaran.IInventoryAutomationConnectionService _inventoryConnectionService;

        private readonly Domain.Aldebaran.ILineService _lineService;
        private readonly Domain.Aldebaran.IMoneyService _moneyService;
        private readonly Domain.Aldebaran.IUnitMeasuredService _unitMeasuredService;
        private readonly Domain.Aldebaran.IItemService _itemService;
        private readonly Domain.Aldebaran.IItemByColorService _itemByColorService;
        private readonly Domain.Aldebaran.ITransitOrderService _transitOrderService;
        private readonly Domain.Aldebaran.IStockService _stockService;
        private readonly Domain.Aldebaran.IPackagingService _packagingService;

        private readonly IMoneySynchronize _moneySync;
        private readonly IUnitMeasuredSynchronize _unitMeasuredSync;
        private readonly ILineSynchronize _lineSync;
        private readonly IItemSynchronize _itemSync;
        private readonly IItemByColorSynchronize _itemByColorSync;
        private readonly ITransitOrderSynchronize _transitSync;
        private readonly IStockSynchronize _stockSync;
        private readonly IPackagingSynchronize _packagingSync;
        private readonly IUpdateProcessSynchronize _updateProcessSync;

        public SyncOrchestratorService(IConfigurator configurator,
            AutomataExistencias.Core.IAutomataState automataState,
            IConnectivityErrorClassifier connectivityClassifier,
            INotificationService notificationService,
            Domain.Aldebaran.IInventoryAutomationConnectionService inventoryConnectionService,
            Domain.Aldebaran.ILineService lineService,
            Domain.Aldebaran.IMoneyService moneyService,
            Domain.Aldebaran.IUnitMeasuredService unitMeasuredService,
            Domain.Aldebaran.IItemService itemService,
            Domain.Aldebaran.IItemByColorService itemByColorService,
            Domain.Aldebaran.ITransitOrderService transitOrderService,
            Domain.Aldebaran.IStockService stockService,
            Domain.Aldebaran.IPackagingService packagingService,
            IMoneySynchronize moneySync,
            IUnitMeasuredSynchronize unitMeasuredSync,
            ILineSynchronize lineSync,
            IItemSynchronize itemSync,
            IItemByColorSynchronize itemByColorSync,
            ITransitOrderSynchronize transitSync,
            IStockSynchronize stockSync,
            IPackagingSynchronize packagingSync,
            IUpdateProcessSynchronize updateProcessSync)
        {
            _configurator = configurator;
            _automataState = automataState;
            _connectivityClassifier = connectivityClassifier;
            _notificationService = notificationService;
            _inventoryConnectionService = inventoryConnectionService;

            _lineService = lineService;
            _moneyService = moneyService;
            _unitMeasuredService = unitMeasuredService;
            _itemService = itemService;
            _itemByColorService = itemByColorService;
            _transitOrderService = transitOrderService;
            _stockService = stockService;
            _packagingService = packagingService;

            _moneySync = moneySync;
            _unitMeasuredSync = unitMeasuredSync;
            _lineSync = lineSync;
            _itemSync = itemSync;
            _itemByColorSync = itemByColorSync;
            _transitSync = transitSync;
            _stockSync = stockSync;
            _packagingSync = packagingSync;
            _updateProcessSync = updateProcessSync;

            _logger = LogManager.GetCurrentClassLogger();
        }

        public void RunOnce(bool ignoreAutomataState = false)
        {
            var syncAttempts = _configurator.GetKey("SyncAttempts").ToInt();

            if (!ignoreAutomataState && (_automataState.IsDestinationConnectivityDown || _automataState.IsOriginConnectivityDown))
            {
                _logger.Warn("Destination or origin connectivity is marked as DOWN. Skipping Sync orchestration execution.");
                return;
            }

            var scheduleSequence = System.Configuration.ConfigurationManager.AppSettings["Schedule.Sequence"].Split(';').ToList();
            var scheduleReverseSequence = System.Configuration.ConfigurationManager.AppSettings["Schedule.Sequence.Reverse"].Split(';').ToList();
            var schedule = scheduleSequence;
            schedule.AddRange(scheduleReverseSequence);

            var lineData = _lineService.Get(syncAttempts);
            var moneyData = _moneyService.Get(syncAttempts);
            var unitMeasuredData = _unitMeasuredService.Get(syncAttempts);
            var itemData = _itemService.Get(syncAttempts);
            var itemByColorData = _itemByColorService.Get(syncAttempts);
            var transitOrderData = _transitOrderService.Get(syncAttempts);
            var stockData = _stockService.Get(syncAttempts);
            var packagingData = _packagingService.Get(syncAttempts);

            foreach (var jobKey in schedule)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                _logger.Info($"[Orchestrator][{jobKey}] has started");
                try
                {
                    switch (jobKey)
                    {
                        case "MoneyJob":
                            _moneySync.Sync(moneyData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "MoneyReverseJob":
                            _moneySync.ReverseSync(moneyData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "UnitMeasuredJob":
                            _unitMeasuredSync.Sync(unitMeasuredData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "UnitMeasuredReverseJob":
                            _unitMeasuredSync.ReverseSync(unitMeasuredData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "LinesJob":
                            _lineSync.Sync(lineData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "LinesReverseJob":
                            _lineSync.ReverseSync(lineData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "ItemsJob":
                            _itemSync.Sync(itemData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "ItemsReverseJob":
                            _itemSync.ReverseSync(itemData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "ItemsByColorJob":
                            _itemByColorSync.Sync(itemByColorData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "ItemsByColorReverseJob":
                            _itemByColorSync.ReverseSync(itemByColorData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "TransitOrderJob":
                            _transitSync.Sync(transitOrderData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "TransitOrderReverseJob":
                            _transitSync.ReverseSync(transitOrderData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "StockJob":
                            _stockSync.Sync(stockData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "StockReverseJob":
                            _stockSync.ReverseSync(stockData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "PackagingJob":
                            _packagingSync.Sync(packagingData.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "PackagingReverseJob":
                            _packagingSync.ReverseSync(packagingData.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)), syncAttempts);
                            break;
                        case "UpdateProcessJob":
                            _updateProcessSync.UpdateProcess();
                            break;
                        default:
                            _logger.Warn($"[Orchestrator] Key [{jobKey}] not identified");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"[Orchestrator] An exception has occurred while execution of {jobKey} | Exception: {ex}");
                    try
                    {
                        if (_connectivityClassifier.IsDestinationConnectivityError(ex.Message))
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
                    _logger.Info($"[Orchestrator][{jobKey}] has finished in {elapsedMs.ToReadableString()}");
                }
            }

            // After running all jobs evaluate connectivity errors like SyncJob
            try
            {
                var windowMinutes = 0;
                int.TryParse(_configurator.GetKey("ConnectivityError.WindowMinutes"), out windowMinutes);
                if (windowMinutes <= 0) windowMinutes = 15;
                var minAttempts = 0;
                int.TryParse(_configurator.GetKey("ConnectivityError.MinAttempts"), out minAttempts);
                if (minAttempts <= 0) minAttempts = 5;
                var percentThreshold = 0.0;
                double.TryParse(_configurator.GetKey("ConnectivityError.PercentThreshold"), out percentThreshold);
                if (percentThreshold <= 0) percentThreshold = 70;

                var totalAttempts = _automataState.GetTotalAttempts(windowMinutes);
                var totalErrors = _automataState.GetConnectivityErrorCount(windowMinutes);
                var pct = _automataState.GetConnectivityErrorPercentage(windowMinutes);

                _logger.Info($"[Orchestrator] Connectivity window {windowMinutes}min: attempts={totalAttempts}, errors={totalErrors}, percent={pct:0.##}%");

                if (totalAttempts >= minAttempts && pct >= percentThreshold)
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

                var consecutiveThreshold = 0;
                int.TryParse(_configurator.GetKey("ConnectivityError.ConsecutiveThreshold"), out consecutiveThreshold);
                if (consecutiveThreshold <= 0) consecutiveThreshold = 10;

                var activeConnections = _inventoryConnectionService.GetActive();
                foreach (var conn in activeConnections)
                {
                    var cons = _automataState.GetConsecutiveFailures(conn.InventoryAutomationConnectionId);
                    if (cons >= consecutiveThreshold)
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
                _logger.Error($"[Orchestrator] Error evaluating connectivity error thresholds: {ex}");
            }
        }
    }
}
