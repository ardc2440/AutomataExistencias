using System;
using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Aldebaran;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class ItemSynchronize : IItemSynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.IItemService _aldebaranItemService;
        private readonly ICatapromDestinationRunner _catapromDestinationRunner;
        private readonly AutomataExistencias.Core.IAutomataState _automataState;
        private readonly AutomataExistencias.Application.IConnectivityErrorClassifier _connectivityErrorClassifier;
        private readonly Domain.Aldebaran.Homologacion.IItemsHomologadosService _itemsHomologadosService;
        private readonly Domain.Aldebaran.Homologacion.ICurrenciesHomologadosService _currenciesHomologadosService;
        private readonly Domain.Aldebaran.Homologacion.IMeasureUnitsHomologadosService _measureUnitsHomologadosService;

        public ItemSynchronize(Domain.Aldebaran.Homologacion.IMeasureUnitsHomologadosService measureUnitsHomologadosService, Domain.Aldebaran.Homologacion.ICurrenciesHomologadosService currenciesHomologadosService, Domain.Aldebaran.IItemService aldebaranItemService, Domain.Aldebaran.Homologacion.IItemsHomologadosService itemsHomologadosService, ICatapromDestinationRunner catapromDestinationRunner, AutomataExistencias.Core.IAutomataState automataState, AutomataExistencias.Application.IConnectivityErrorClassifier connectivityErrorClassifier)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranItemService = aldebaranItemService;
            _catapromDestinationRunner = catapromDestinationRunner;
            _automataState = automataState;
            _connectivityErrorClassifier = connectivityErrorClassifier;
            _itemsHomologadosService = itemsHomologadosService;
            _currenciesHomologadosService = currenciesHomologadosService;
            _measureUnitsHomologadosService = measureUnitsHomologadosService;
        }
        public void Sync(IEnumerable<Item> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(i => i.Id).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [ItemsSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Aldebaran to Cataprom [ItemsSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var itemHomologado = _itemsHomologadosService.GetById(item.ItemId);
                        var currencyHomologado = _currenciesHomologadosService.GetById((short)item.MoneyId);
                        var fobMeasureUnitHomologado = _measureUnitsHomologadosService.GetById((short)item.FobUnitId);
                        var cifMeasureUnitHomologado = _measureUnitsHomologadosService.GetById((short)item.CifUnitId);

                        var catapromItemService = new Domain.Cataprom.ItemService(unitOfWorkCataprom);
                        catapromItemService.AddOrUpdate(new DataAccess.Cataprom.Item
                        {
                            Id = itemHomologado.ItemIdHomologado,
                            LineId = itemHomologado.LineIdHomologada,
                            Reference = item.Reference,
                            Name = item.Name,
                            ProviderReference = item.ProviderReference,
                            ProviderItemName = item.ProviderItemName,
                            ItemType = item.ItemType,
                            FobCost = (decimal)item.FobCost.NullTo(),
                            MoneyId = currencyHomologado.CurrencyIdHomologado,
                            PartType = item.PartType,
                            Determinant = item.Determinant,
                            Observations = item.Observations,
                            StockExt = item.StockExt,
                            CifCost = item.CifCost,
                            Volume = (decimal)item.Volume,
                            Weight = (decimal)item.Weight,
                            FobUnitId = fobMeasureUnitHomologado.MeasureUnitIdHomologado,
                            CifUnitId = cifMeasureUnitHomologado.MeasureUnitIdHomologado,
                            NationalProduct = item.NationalProduct,
                            Active = item.Active,
                            VisibleCatalog = item.VisibleCatalog,
                        });
                        catapromItemService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to insert/update an Item from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update an Item from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");

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
                        _aldebaranItemService.Remove(item);
                        inserted++;
                    }
                    else
                    {
                        _aldebaranItemService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranItemService.SaveChanges();
                }
            }

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Item sql table");
        }
        public void ReverseSync(IEnumerable<Item> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(i => i.Id).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [ItemsReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Aldebaran to Cataprom [ItemsReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var itemHomologado = _itemsHomologadosService.GetById(item.ItemId);

                        var catapromItemService = new Domain.Cataprom.ItemService(unitOfWorkCataprom);
                        catapromItemService.Remove(new DataAccess.Cataprom.Item { Id = itemHomologado.ItemIdHomologado });
                        catapromItemService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to delete an Item from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete an Item from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");

                        try
                        {
                            if (_connectivityErrorClassifier.IsDestinationConnectivityError(item.Exception))
                                _automataState.IncrementConnectivityError("Item", connection.InventoryAutomationConnectionId);
                        }
                        catch { }
                    }
                });

                try
                {
                    if (allDestinationsOk)
                    {
                        _aldebaranItemService.Remove(item);
                        deleted++;
                    }
                    else
                    {
                        _aldebaranItemService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranItemService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Item sql table");
        }
    }
}
