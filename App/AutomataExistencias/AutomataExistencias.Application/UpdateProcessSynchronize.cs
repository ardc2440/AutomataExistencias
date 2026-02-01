using System;
using AutomataExistencias.Core.Extensions;
using NLog;
using System.Data.Entity;

namespace AutomataExistencias.Application
{
    // Run the update process on every Cataprom destination (multidestino)
    public class UpdateProcessSynchronize : IUpdateProcessSynchronize
    {
        private readonly Logger _logger;
        private readonly ICatapromDestinationRunner _catapromDestinationRunner;
        private readonly Core.IAutomataState _automataState;
        private readonly IConnectivityErrorClassifier _connectivityErrorClassifier;

        public UpdateProcessSynchronize(ICatapromDestinationRunner catapromDestinationRunner,
            Core.IAutomataState automataState,
            IConnectivityErrorClassifier connectivityErrorClassifier)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _catapromDestinationRunner = catapromDestinationRunner;
            _automataState = automataState;
            _connectivityErrorClassifier = connectivityErrorClassifier;
        }

        public void UpdateProcess()
        {
            _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
            {
                try
                {
                    var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                    _logger.Info($"Updating LastImportDate on destination {connInfo}");

                    // Execute update via EF context
                    _logger.Info($"Attempting LastImportDate update for ConnId={connection.InventoryAutomationConnectionId}");
                    var db = unitOfWorkCataprom.GetContext().Database;
                    db.ExecuteSqlCommand("UPDATE tbl_rActualizacion SET fechaActualizacion=GETDATE()");

                    _logger.Info($"LastImportDate update succeeded for ConnId={connection.InventoryAutomationConnectionId}");

                    // Record a successful attempt for this connection so aggregated metrics include it
                    try
                    {
                        _automataState.RecordAttempt(connection.InventoryAutomationConnectionId, false);
                        _logger.Debug($"Recorded successful attempt for ConnId={connection.InventoryAutomationConnectionId}");
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    try
                    {
                        _logger.Error($"Error updating LastImportDate for connection {connection.InventoryAutomationConnectionId} | Exception: {ex.ToJson()}");
                    }
                    catch
                    {
                        _logger.Error(ex.ToString());
                    }

                    try
                    {
                        // If exception looks like connectivity-related, count it in AutomataState for this connection
                        var exText = ex.ToString();
                        var isConn = false;
                        try { isConn = _connectivityErrorClassifier.IsDestinationConnectivityError(exText); } catch { isConn = false; }
                        try
                        {
                            _automataState.RecordAttempt(connection.InventoryAutomationConnectionId, isConn);
                            _logger.Info($"Recorded attempt for ConnId={connection.InventoryAutomationConnectionId} isConnectivityError={isConn}");
                        }
                        catch { }
                    }
                    catch { }
                }
            });
        }
    }
}
