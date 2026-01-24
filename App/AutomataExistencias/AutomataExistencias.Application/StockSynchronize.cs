using System;
using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Aldebaran;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class StockSynchronize : IStockSynchronize
    {
        private readonly Logger _logger;
        /*Aldebaran*/
        private readonly Domain.Aldebaran.IStockService _aldebaranStockService;
        private readonly ICatapromDestinationRunner _catapromDestinationRunner;
        private readonly Domain.Aldebaran.Homologacion.IItemReferencesHomologadosService _itemReferencesHomologadosService;
        private readonly AutomataExistencias.Core.IAutomataState _automataState;
        private readonly AutomataExistencias.Application.IConnectivityErrorClassifier _connectivityErrorClassifier;

        public StockSynchronize(Domain.Aldebaran.Homologacion.IItemReferencesHomologadosService itemReferencesHomologadosService, Domain.Aldebaran.IStockService aldebaranStockService, ICatapromDestinationRunner catapromDestinationRunner, AutomataExistencias.Core.IAutomataState automataState, AutomataExistencias.Application.IConnectivityErrorClassifier connectivityErrorClassifier)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranStockService = aldebaranStockService;
            _catapromDestinationRunner = catapromDestinationRunner;
            _itemReferencesHomologadosService = itemReferencesHomologadosService;
            _automataState = automataState;
            _connectivityErrorClassifier = connectivityErrorClassifier;
        }
        public void Sync(IEnumerable<Stock> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(s => s.Id).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [StockSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Aldebaran to Cataprom [StockSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var itemReferencesHomologado = _itemReferencesHomologadosService.GetById(item.ColorItemId);

                        var catapromStockService = new Domain.Cataprom.StockService(unitOfWorkCataprom);
                        catapromStockService.AddOrUpdate(new DataAccess.Cataprom.Stock
                        {
                            ColorItemId = itemReferencesHomologado.ReferenceIdHomologado,
                            ItemId = itemReferencesHomologado.ItemIdHomologado,
                            Color = item.Color,
                            Quantity = item.Quantity,
                            StorageCellar = item.StorageCellar
                        });
                        catapromStockService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to insert/update a Stock from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update a Stock from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");

                        try
                        {
                            var isConn = _connectivityErrorClassifier.IsDestinationConnectivityError(item.Exception);
                            _automataState.RecordAttempt(connection.InventoryAutomationConnectionId, isConn);
                        }
                        catch { }
                    }
                });

                try
                {
                    if (allDestinationsOk)
                    {
                        _aldebaranStockService.Remove(item);
                        inserted++;
                    }
                    else
                    {
                        _aldebaranStockService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranStockService.SaveChanges();
                }
            }

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Stock sql table");
        }
        public void ReverseSync(IEnumerable<Stock> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(s => s.Id).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [StockReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Aldebaran to Cataprom [StockReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var itemReferencesHomologado = _itemReferencesHomologadosService.GetById(item.ColorItemId);

                        var catapromStockService = new Domain.Cataprom.StockService(unitOfWorkCataprom);
                        catapromStockService.Remove(new DataAccess.Cataprom.Stock
                        {
                            ColorItemId = itemReferencesHomologado.ReferenceIdHomologado,
                            StorageCellar = item.StorageCellar
                        });
                        catapromStockService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to delete a Stock from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete a Stock from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    }
                });

                try
                {
                    if (allDestinationsOk)
                    {
                        _aldebaranStockService.Remove(item);
                        deleted++;
                    }
                    else
                    {
                        _aldebaranStockService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranStockService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Stock sql table");
        }
    }
}
