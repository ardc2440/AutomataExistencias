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
        private readonly AutomataExistencias.Core.Configuration.IConfigurator _configurator;
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
            _configurator = configurator;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public bool TryRecoverOnce()
        {
            try
            {
                _logger.Info("RecoveryService: starting single recovery attempt");

                // Strategy: for each synchronizable entity, collect pending rows whose Exception classifies as connectivity error
                var syncAttempts = 0;
                int.TryParse(_configurator.GetKey("SyncAttempts"), out syncAttempts);
                if (syncAttempts <= 0) syncAttempts = 1;

                var totalProcessed = 0;

                // Items
                var items = _itemService.Get().Where(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception)).ToList();
                if (items.Any())
                {
                    _logger.Info($"RecoveryService: attempting recovery for {items.Count} Item records");
                    _itemSynchronize.Sync(items, syncAttempts);
                    totalProcessed += items.Count;
                }

                // Stock
                var stocks = _stockService.Get().Where(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception)).ToList();
                if (stocks.Any())
                {
                    _logger.Info($"RecoveryService: attempting recovery for {stocks.Count} Stock records");
                    _stockSynchronize.Sync(stocks, syncAttempts);
                    totalProcessed += stocks.Count;
                }

                // Packaging
                var packs = _packagingService.Get().Where(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception)).ToList();
                if (packs.Any())
                {
                    _logger.Info($"RecoveryService: attempting recovery for {packs.Count} Packaging records");
                    _packagingSynchronize.Sync(packs, syncAttempts);
                    totalProcessed += packs.Count;
                }

                // TransitOrder
                var trans = _transitOrderService.Get().Where(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception)).ToList();
                if (trans.Any())
                {
                    _logger.Info($"RecoveryService: attempting recovery for {trans.Count} TransitOrder records");
                    _transitOrderSynchronize.Sync(trans, syncAttempts);
                    totalProcessed += trans.Count;
                }

                // ItemByColor
                var bycolors = _itemByColorService.Get().Where(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception)).ToList();
                if (bycolors.Any())
                {
                    _logger.Info($"RecoveryService: attempting recovery for {bycolors.Count} ItemByColor records");
                    _itemByColorSynchronize.Sync(bycolors, syncAttempts);
                    totalProcessed += bycolors.Count;
                }

                // Money and UnitMeasured optional
                // money/unitMeasured recovery not implemented in this pass
                var monies = new System.Collections.Generic.List<object>();

                // After attempts, check remaining connectivity pendings
                var remaining = 0;
                remaining += _itemService.Get().Count(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception));
                remaining += _stockService.Get().Count(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception));
                remaining += _packagingService.Get().Count(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception));
                remaining += _transitOrderService.Get().Count(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception));
                remaining += _itemByColorService.Get().Count(i => !string.IsNullOrEmpty(i.Exception) && _connectivityErrorClassifier.IsDestinationConnectivityError(i.Exception));

                if (remaining == 0)
                {
                    _logger.Info($"RecoveryService: recovery succeeded, cleared remaining connectivity pendings");
                    _automataState.ResetConnectivityErrorCounts();
                    _automataState.IsDestinationConnectivityDown = false;
                    _automataState.DestinationConnectivityDownSince = null;
                    return true;
                }

                _logger.Warn($"RecoveryService: recovery finished but {remaining} connectivity pendings remain");
                var connections = _inventoryConnectionService.GetActive();
                _notificationService.NotifyConnectivityDown(connections, DateTime.UtcNow);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error($"RecoveryService error: {ex}");
                return false;
            }
        }
    }
}
