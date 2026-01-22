using System;
using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Aldebaran;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class TransitOrderSynchronize : ITransitOrderSynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.ITransitOrderService _aldebaranTransitOrderService;
        private readonly ICatapromDestinationRunner _catapromDestinationRunner;
        private readonly Domain.Aldebaran.Homologacion.IItemReferencesHomologadosService _itemReferencesHomologadosService;

        public TransitOrderSynchronize(Domain.Aldebaran.Homologacion.IItemReferencesHomologadosService itemReferencesHomologadosService, Domain.Aldebaran.ITransitOrderService aldebaranTransitOrderService, ICatapromDestinationRunner catapromDestinationRunner)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranTransitOrderService = aldebaranTransitOrderService;
            _catapromDestinationRunner = catapromDestinationRunner;
            _itemReferencesHomologadosService = itemReferencesHomologadosService;
        }
        public void Sync(IEnumerable<TransitOrder> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(t => t.Id).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [UnitMeasuredSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Aldebaran to Cataprom [UnitMeasuredSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var itemReferencesHomologado = _itemReferencesHomologadosService.GetById((int)item.ColorItemId);

                        var catapromTransitOrderService = new Domain.Cataprom.TransitOrderService(unitOfWorkCataprom);
                        catapromTransitOrderService.AddOrUpdate(new DataAccess.Cataprom.TransitOrder
                        {
                            Id = item.TransitOrderItemId,
                            DeliveredDate = item.DeliveredDate.NullTo(DateTime.Now),
                            DeliveredQuantity = item.DeliveredQuantity.NullTo(),
                            Date = item.Date.NullTo(DateTime.Now),
                            Activity = item.Activity,
                            ColorItemId = itemReferencesHomologado.ReferenceIdHomologado
                        });
                        catapromTransitOrderService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to insert/update a TransitOrder from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update a TransitOrder from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    }
                });

                try
                {
                    if (allDestinationsOk)
                    {
                        _aldebaranTransitOrderService.Remove(item);
                        inserted++;
                    }
                    else
                    {
                        _aldebaranTransitOrderService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranTransitOrderService.SaveChanges();
                }
            }

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from TransitOrder sql table");
        }
        public void ReverseSync(IEnumerable<TransitOrder> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(t => t.Id).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [UnitMeasuredReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Aldebaran to Cataprom [UnitMeasuredReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var catapromTransitOrderService = new Domain.Cataprom.TransitOrderService(unitOfWorkCataprom);
                        catapromTransitOrderService.Remove(new DataAccess.Cataprom.TransitOrder { Id = item.TransitOrderItemId });
                        catapromTransitOrderService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to delete a TransitOrder from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete a TransitOrder from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    }
                });

                try
                {
                    if (allDestinationsOk)
                    {
                        _aldebaranTransitOrderService.Remove(item);
                        deleted++;
                    }
                    else
                    {
                        _aldebaranTransitOrderService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranTransitOrderService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from TransitOrder sql table");
        }
    }
}