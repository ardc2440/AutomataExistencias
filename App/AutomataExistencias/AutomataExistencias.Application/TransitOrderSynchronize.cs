using System;
using System.Linq;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class TransitOrderSynchronize : ITransitOrderSynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.ITransitOrderService _aldebaranTransitOrderService;
        private readonly Domain.Cataprom.ITransitOrderService _catapromTransitOrderService;
        private readonly int _syncAttempts;

        public TransitOrderSynchronize(Domain.Aldebaran.ITransitOrderService aldebaranTransitOrderService, Domain.Cataprom.ITransitOrderService catapromTransitOrderService, IConfigurator configurator)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranTransitOrderService = aldebaranTransitOrderService;
            _catapromTransitOrderService = catapromTransitOrderService;
            _syncAttempts = configurator.GetKey("SyncAttempts").ToInt();
        }
        public void Sync()
        {
            var dataFirebird = _aldebaranTransitOrderService.Get(_syncAttempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Firebird to Sql [UnitMeasuredSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Firebird to Sql [UnitMeasuredSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromTransitOrderService.AddOrUpdate(new DataAccess.Cataprom.TransitOrder
                    {
                        Id = item.TransitOrderItemId,
                        DeliveredDate = item.DeliveredDate.NullTo(DateTime.Now),
                        DeliveredQuantity = item.DeliveredQuantity.NullTo(),
                        Date = item.Date.NullTo(DateTime.Now),
                        Activity = item.Activity,
                        ColorItemId = item.ColorItemId.NullTo(),
                    });
                    _catapromTransitOrderService.SaveChanges();
                    _aldebaranTransitOrderService.Remove(item);
                    inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to insert/update a TransitOrder from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to insert/update a TransitOrder from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranTransitOrderService.Update(item);
                }
                finally
                {
                    _aldebaranTransitOrderService.SaveChanges();
                }
            }

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from TransitOrder sql table");
        }
        public void ReverseSync()
        {
            var dataFirebird = _aldebaranTransitOrderService.Get(_syncAttempts).Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Firebird to Sql [UnitMeasuredReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Firebird to Sql [UnitMeasuredReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromTransitOrderService.Remove(new DataAccess.Cataprom.TransitOrder { Id = item.TransitOrderItemId });
                    _catapromTransitOrderService.SaveChanges();
                    _aldebaranTransitOrderService.Remove(item);
                    deleted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to delete a TransitOrder from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to delete a TransitOrder from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranTransitOrderService.Update(item);
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