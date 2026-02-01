using System;
using System.Linq;
using AutomataExistencias.Domain.Aldebaran;
using AutomataExistencias.Core.Extensions;
using NLog;

namespace AutomataExistencias.Application
{
    public class StartupRecoveryChecker : IStartupRecoveryChecker
    {
        private readonly IItemService _itemService;
        private readonly IStockService _stockService;
        private readonly IPackagingService _packagingService;
        private readonly ITransitOrderService _transitOrderService;
        private readonly IItemByColorService _itemByColorService;
        private readonly IMoneyService _moneyService;
        private readonly IUnitMeasuredService _unitMeasuredService;
        private readonly IConnectivityErrorClassifier _connectivityErrorClassifier;
        private readonly Core.IAutomataState _automataState;
        private readonly INotificationService _notificationService;
        private readonly Core.Configuration.IConfigurator _configurator;
        private readonly Logger _logger;
        private readonly IRecoveryService _recoveryService;

        private readonly IInventoryAutomationConnectionService _inventoryConnectionService;

        public StartupRecoveryChecker(IMoneyService moneyService, IUnitMeasuredService unitMeasuredService, IItemService itemService, IStockService stockService, IPackagingService packagingService,
            ITransitOrderService transitOrderService, IItemByColorService itemByColorService,
            IConnectivityErrorClassifier connectivityErrorClassifier, Core.IAutomataState automataState,
            INotificationService notificationService, Core.Configuration.IConfigurator configurator,
            IInventoryAutomationConnectionService inventoryConnectionService,
            IRecoveryService recoveryService)
        {
            _moneyService = moneyService;
            _unitMeasuredService = unitMeasuredService;
            _itemService = itemService;
            _stockService = stockService;
            _packagingService = packagingService;
            _transitOrderService = transitOrderService;
            _itemByColorService = itemByColorService;
            _connectivityErrorClassifier = connectivityErrorClassifier;
            _automataState = automataState;
            _notificationService = notificationService;
            _configurator = configurator;
            _inventoryConnectionService = inventoryConnectionService;
            _recoveryService = recoveryService;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public bool ShouldRunRecoveryOnStartup()
        {
            try
            { 
                var minAttempts = ParseIntOrDefault("ConnectivityError.MinAttempts", 5);
                var percentThreshold = ParseDoubleOrDefault("ConnectivityError.PercentThreshold", 70);

                // Count pending items across main tables
                // Fallback: count pending items across main tables (legacy behavior)
                var itemPend = _itemService.Get().Count(i => !string.IsNullOrEmpty(i.Exception));
                var stockPend = _stockService.Get().Count(i => !string.IsNullOrEmpty(i.Exception));
                var packPend = _packagingService.Get().Count(i => !string.IsNullOrEmpty(i.Exception));
                var transitPend = _transitOrderService.Get().Count(i => !string.IsNullOrEmpty(i.Exception));
                var itemByColorPend = _itemByColorService.Get().Count(i => !string.IsNullOrEmpty(i.Exception));

                var totalPend = itemPend + stockPend + packPend + transitPend + itemByColorPend;
                if (totalPend < minAttempts) 
                {
                    _logger.Info($"StartupRecoveryChecker: total pending {totalPend} < minAttempts {minAttempts}. No recovery needed.");
                    return false;
                }

                // Debug: log samples of exception texts per table to verify classifier behavior
                try
                {
                    // Use reflection to optionally filter by FECHA_INTEGRA/FechaIntegra if available on the domain objects
                    const int sampleLimit = 5;
                    var items = _itemService.Get().Where(i => !string.IsNullOrEmpty(i.Exception)).ToList();
                    _logger.Info($"Startup debug: Item pending={items.Count}");
                    foreach (var exText in items.Take(sampleLimit).Select(i => i.Exception))
                        _logger.Info($"Item exception sample (isConn={_connectivityErrorClassifier.IsDestinationConnectivityError(exText)}): {exText}");

                    var stocks = _stockService.Get().Where(i => !string.IsNullOrEmpty(i.Exception)).ToList();
                    _logger.Info($"Startup debug: Stock pending={stocks.Count}");
                    foreach (var exText in stocks.Take(sampleLimit).Select(i => i.Exception))
                        _logger.Info($"Stock exception sample (isConn={_connectivityErrorClassifier.IsDestinationConnectivityError(exText)}): {exText}");

                    var packs = _packagingService.Get().Where(i => !string.IsNullOrEmpty(i.Exception)).ToList();
                    _logger.Info($"Startup debug: Packaging pending={packs.Count}");
                    foreach (var exText in packs.Take(sampleLimit).Select(i => i.Exception))
                        _logger.Info($"Packaging exception sample (isConn={_connectivityErrorClassifier.IsDestinationConnectivityError(exText)}): {exText}");

                    var transits = _transitOrderService.Get().Where(i => !string.IsNullOrEmpty(i.Exception)).ToList();
                    _logger.Info($"Startup debug: Transit pending={transits.Count}");
                    foreach (var exText in transits.Take(sampleLimit).Select(i => i.Exception))
                        _logger.Info($"Transit exception sample (isConn={_connectivityErrorClassifier.IsDestinationConnectivityError(exText)}): {exText}");

                    var bycolors = _itemByColorService.Get().Where(i => !string.IsNullOrEmpty(i.Exception)).ToList();
                    _logger.Info($"Startup debug: ItemByColor pending={bycolors.Count}");
                    foreach (var exText in bycolors.Take(sampleLimit).Select(i => i.Exception))
                        _logger.Info($"ItemByColor exception sample (isConn={_connectivityErrorClassifier.IsDestinationConnectivityError(exText)}): {exText}");
                }
                catch { }

                // classify connectivity errors
                var connPend = _itemService.Get().Count(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception));
                connPend += _stockService.Get().Count(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception));
                connPend += _packagingService.Get().Count(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception));

                var pct = (connPend * 100.0) / totalPend;
                _logger.Info($"StartupRecoveryChecker: pending total={totalPend}, connectivity={connPend}, pct={pct:0.##}% (threshold={percentThreshold}%)");

                return pct >= percentThreshold; 
            }
            catch (Exception ex)
            {
                _logger.Error($"StartupRecoveryChecker error: {ex.ToJson()}");
                // In doubt, do not block startup
                return false;
            }
        }

        public bool TryRunRecoveryOnStartup()
        {
            // Minimal behavior: notify recipients and return false (meaning recovery not completed)
            try
            {
                _logger.Info("StartupRecoveryChecker: triggering notification for startup recovery");
                return _recoveryService.TryRecoverOnce();
            }
            catch (Exception ex)
            {
                _logger.Error($"StartupRecoveryChecker TryRunRecoveryOnStartup error: {ex.ToJson()}");
                return false;
            }
        }

        // Try to extract InnerException message from serialized JSON exception text
        private string ExtractInnerException(string exText)
        {
            try
            {
                if (string.IsNullOrEmpty(exText)) return exText;
                // crude check for JSON-like content
                if (exText.TrimStart().StartsWith("{") && exText.Contains("InnerException"))
                {
                    // attempt to find "InnerException":"..." pattern
                    var marker = "\"InnerException\":";
                    var idx = exText.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                    if (idx >= 0)
                    {
                        var start = idx + marker.Length;
                        // skip optional whitespace and opening quote
                        while (start < exText.Length && (exText[start] == ' ' || exText[start] == '\\' || exText[start] == '"')) start++;
                        var end = exText.IndexOf('"', start);
                        if (end > start)
                        {
                            var inner = exText.Substring(start, end - start);
                            return inner;
                        }
                    }
                }
            }
            catch { }
            return exText;
        }

        private int ParseIntOrDefault(string key, int def)
        {
            if (int.TryParse(_configurator.GetKey(key), out var v)) return v;
            return def;
        }

        private double ParseDoubleOrDefault(string key, double def)
        {
            if (double.TryParse(_configurator.GetKey(key), out var v)) return v;
            return def;
        }
    }
}
