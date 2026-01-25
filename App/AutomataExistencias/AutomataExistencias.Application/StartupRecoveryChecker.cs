using System;
using System.Linq;
using AutomataExistencias.Domain.Aldebaran;
using AutomataExistencias.Application;
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
        private readonly IConnectivityErrorClassifier _connectivityErrorClassifier;
        private readonly AutomataExistencias.Core.IAutomataState _automataState;
        private readonly AutomataExistencias.Application.INotificationService _notificationService;
        private readonly AutomataExistencias.Core.Configuration.IConfigurator _configurator;
        private readonly Logger _logger;
        private readonly IRecoveryService _recoveryService;

        private readonly Domain.Aldebaran.IInventoryAutomationConnectionService _inventoryConnectionService;

        public StartupRecoveryChecker(IItemService itemService, IStockService stockService, IPackagingService packagingService,
            ITransitOrderService transitOrderService, IItemByColorService itemByColorService,
            IConnectivityErrorClassifier connectivityErrorClassifier, AutomataExistencias.Core.IAutomataState automataState,
            INotificationService notificationService, AutomataExistencias.Core.Configuration.IConfigurator configurator,
            Domain.Aldebaran.IInventoryAutomationConnectionService inventoryConnectionService,
            IRecoveryService recoveryService)
        {
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
