using System;
using System.Linq;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class ItemByColorSynchronize : IItemByColorSynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.IItemByColorService _aldebaranItemByColorService;
        private readonly Domain.Cataprom.IItemByColorService _catapromItemByColorService;
        private readonly int _syncAttempts;
        public ItemByColorSynchronize(Domain.Aldebaran.IItemByColorService aldebaranItemByColorService, Domain.Cataprom.IItemByColorService catapromItemByColorService, IConfigurator configurator)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranItemByColorService = aldebaranItemByColorService;
            _catapromItemByColorService = catapromItemByColorService;
            _syncAttempts = configurator.GetKey("SyncAttempts").ToInt();
        }
        public void Sync()
        {
            var dataFirebird = _aldebaranItemByColorService.Get(_syncAttempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Firebird to Sql [ItemsByColorSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Firebird to Sql [ItemsByColorSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromItemByColorService.AddOrUpdate(new DataAccess.Cataprom.ItemByColor
                    {
                        Id = item.ColorItemId,
                        ItemId = item.ItemId.NullTo(),
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
                        SoldOut = item.SoldOut,
                        QuantityProcess = item.QuantityProcess.NullTo(),
                    });
                    _catapromItemByColorService.SaveChanges();
                    _aldebaranItemByColorService.Remove(item);
                    inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to insert/update an ItemByColor from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to insert/update an ItemByColor from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranItemByColorService.Update(item);
                }
                finally
                {
                    _aldebaranItemByColorService.SaveChanges();
                }
            }

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from ItemByColor sql table");
        }
        public void ReverseSync()
        {
            var dataFirebird = _aldebaranItemByColorService.Get(_syncAttempts).Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Firebird to Sql [ItemsByColorReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Firebird to Sql [ItemsByColorReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromItemByColorService.Remove(new DataAccess.Cataprom.ItemByColor { Id = item.ColorItemId });
                    _catapromItemByColorService.SaveChanges();
                    _aldebaranItemByColorService.Remove(item);
                    deleted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to delete an ItemByColor from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to delete an ItemByColor from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranItemByColorService.Update(item);
                }
                finally
                {
                    _aldebaranItemByColorService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from ItemByColor sql table");
        }
    }
}
