using System;
using AutomataExistencias.DataAccess.Core;
using AutomataExistencias.DataAccess.Core.Contract;
using AutomataExistencias.Domain.Aldebaran;
using AutomataExistencias.DataAccess.Aldebaran;
using NLog;
using AutomataExistencias.Core.Extensions;

namespace AutomataExistencias.Application
{
    public class CatapromDestinationRunner : ICatapromDestinationRunner
    {
        private readonly IInventoryAutomationConnectionService _inventoryConnectionService;
        private readonly Logger _logger;
        private readonly Core.IAutomataState _automataState;
        private readonly IConnectivityErrorClassifier _connectivityErrorClassifier;

        public CatapromDestinationRunner(IInventoryAutomationConnectionService inventoryConnectionService, Core.IAutomataState automataState, IConnectivityErrorClassifier connectivityErrorClassifier)
        {
            _inventoryConnectionService = inventoryConnectionService;
            _automataState = automataState;
            _connectivityErrorClassifier = connectivityErrorClassifier;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void RunForAllDestinations(Action<IUnitOfWorkCataprom, InventoryAutomationConnection> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var connections = _inventoryConnectionService.GetActive();
            foreach (var connection in connections)
            {
                try
                {
                    var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                    _logger.Info($"Running action for Cataprom destination {connInfo}");

                    var connectionString = InventoryAutomationConnectionStringBuilder.Build(connection);
                    using (var context = new CatapromBaseContext(connectionString))
                    {
                        var unitOfWork = new UnitOfWorkCataprom(context);
                        action(unitOfWork, connection);
                    }
                }
                catch (Exception ex)
                {
                    try { _logger.Error($"Error running action for connection Id {connection.InventoryAutomationConnectionId} | Exception: {ex.ToJson()}"); } catch { _logger.Error(ex.ToString()); }
                    try
                    {
                        var exText = ex.ToString();
                        var isConn = false;
                        try { isConn = _connectivityErrorClassifier.IsDestinationConnectivityError(exText); } catch { isConn = false; }
                        if (isConn)
                        {
                            try { _automataState.RecordAttempt(connection.InventoryAutomationConnectionId, true); } catch { }
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
