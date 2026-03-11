using System;
using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Aldebaran;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class ItemByColorSynchronize : IItemByColorSynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.IItemByColorService _aldebaranItemByColorService;
        private readonly ICatapromDestinationRunner _catapromDestinationRunner;
        private readonly Domain.Aldebaran.Homologacion.IItemReferencesHomologadosService _itemReferencesHomologadosService;
        private readonly Core.IAutomataState _automataState;
        private readonly IConnectivityErrorClassifier _connectivityErrorClassifier;

        public ItemByColorSynchronize(Domain.Aldebaran.Homologacion.IItemReferencesHomologadosService itemReferencesHomologadosService, Domain.Aldebaran.IItemByColorService aldebaranItemByColorService, ICatapromDestinationRunner catapromDestinationRunner, Core.IAutomataState automataState, IConnectivityErrorClassifier connectivityErrorClassifier)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranItemByColorService = aldebaranItemByColorService;
            _catapromDestinationRunner = catapromDestinationRunner;
            _itemReferencesHomologadosService = itemReferencesHomologadosService;
            _automataState = automataState;
            _connectivityErrorClassifier = connectivityErrorClassifier;
        }
        public void Sync(IEnumerable<ItemByColor> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(i => i.Id);
            var inserted = 0;
            var processed = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var itemReferenceHomologado = _itemReferencesHomologadosService.GetById(item.ColorItemId);

                        var catapromItemByColorService = new Domain.Cataprom.ItemByColorService(unitOfWorkCataprom);
                        catapromItemByColorService.AddOrUpdate(new DataAccess.Cataprom.ItemByColor
                        {
                            Id = itemReferenceHomologado.ReferenceIdHomologado,
                            ItemId = itemReferenceHomologado.ItemIdHomologado,
                            ItemByColorReference = item.ItemByColorReference,
                            ItemByColorInternalReference = item.ItemByColorInternalReference,
                            ColorName = item.ColorName,
                            ProviderNomItemByColor = item.ProviderNomItemByColor,
                            Observations = item.Observations,
                            Color = item.Color,
                            QuantityOrder = item.QuantityOrder.NullTo(),
                            Quantity = item.Quantity.NullTo(),
                            QuantityReserved = item.QuantityReserved.NullTo(),
                            QuantityOrderPan = item.QuantityOrderPan.NullTo(),
                            QuantityPan = item.QuantityPan.NullTo(),
                            QuantityReservedPan = item.QuantityReservedPan.NullTo(),
                            Active = item.Active,
                            SoldOut = item.SoldOut,
                            QuantityProcess = item.QuantityProcess.NullTo(),
                        });
                        catapromItemByColorService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to insert/update an ItemByColor from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: Id={item.Id},ColorItemId={item.ColorItemId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                        else
                        {
                            var full = SerializationThrottler.SerializeIfAllowed(item, "ItemByColorSync");
                            if (!string.IsNullOrEmpty(full))
                                _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update an ItemByColor from Aldebaran to Cataprom. | FullData: {full} | Exception: {ex.ToJson()}");
                            else
                                _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update an ItemByColor from Aldebaran to Cataprom. | Data: Id={item.Id},ColorItemId={item.ColorItemId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                        }
                        try
                        {
                            var exText = ex.ToString();
                            var isConn = _connectivityErrorClassifier.IsDestinationConnectivityError(exText);
                            _automataState.RecordAttempt(connection.InventoryAutomationConnectionId, isConn);
                        }
                        catch { }
                    }
                });

                try
                {
                    processed++;
                    if (allDestinationsOk)
                    {
                        _aldebaranItemByColorService.Remove(item);
                        inserted++;
                    }
                    else
                    {
                        _aldebaranItemByColorService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranItemByColorService.SaveChanges();
                }
            }
            if (processed == 0)
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [ItemsByColorSync]");
                return;
            }

            _logger.Info($"Found {processed} records to insert/update from Aldebaran to Cataprom [ItemsByColorSync]");

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from ItemByColor sql table");
        }
        public void ReverseSync(IEnumerable<ItemByColor> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(i => i.Id);
            var deleted = 0;
            var processed = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var itemReferenceHomologado = _itemReferencesHomologadosService.GetById(item.ColorItemId);

                        var catapromItemByColorService = new Domain.Cataprom.ItemByColorService(unitOfWorkCataprom);
                        catapromItemByColorService.Remove(new DataAccess.Cataprom.ItemByColor { Id = itemReferenceHomologado.ReferenceIdHomologado });
                        catapromItemByColorService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to delete an ItemByColor from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: Id={item.Id},ColorItemId={item.ColorItemId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete an ItemByColor from Aldebaran to Cataprom. | Data: Id={item.Id},ColorItemId={item.ColorItemId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                    }
                });

                try
                {
                    if (allDestinationsOk)
                    {
                        _aldebaranItemByColorService.Remove(item);
                        deleted++;
                    }
                    else
                    {
                        _aldebaranItemByColorService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranItemByColorService.SaveChanges();
                }
                processed++;
            }
            if (processed == 0)
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [ItemsByColorReverseSync]");
                return;
            }

            _logger.Info($"Found {processed} records to delete from Aldebaran to Cataprom [ItemsByColorReverseSync]");

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from ItemByColor sql table");
        }
    }
}
