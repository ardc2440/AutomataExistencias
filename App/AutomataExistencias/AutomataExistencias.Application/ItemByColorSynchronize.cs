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
        private readonly Domain.Cataprom.IItemByColorService _catapromItemByColorService;
        private readonly Domain.Aldebaran.Homologacion.IItemReferencesHomologadosService _itemReferencesHomologadosService;

        public ItemByColorSynchronize(Domain.Aldebaran.Homologacion.IItemReferencesHomologadosService itemReferencesHomologadosService, Domain.Aldebaran.IItemByColorService aldebaranItemByColorService, Domain.Cataprom.IItemByColorService catapromItemByColorService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranItemByColorService = aldebaranItemByColorService;
            _catapromItemByColorService = catapromItemByColorService;
            _itemReferencesHomologadosService = itemReferencesHomologadosService;
        }
        public void Sync(IEnumerable<ItemByColor> data, int syncAttempts)
        {
            var dataFirebird = data.ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [ItemsByColorSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Aldebaran to Cataprom [ItemsByColorSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    var itemReferenceHomologado = _itemReferencesHomologadosService.GetById(item.ColorItemId);

                    _catapromItemByColorService.AddOrUpdate(new DataAccess.Cataprom.ItemByColor
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
                    _catapromItemByColorService.SaveChanges();
                    _aldebaranItemByColorService.Remove(item);
                    inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to insert/update an ItemByColor from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update an ItemByColor from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
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
        public void ReverseSync(IEnumerable<ItemByColor> data, int syncAttempts)
        {
            var dataFirebird = data.ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [ItemsByColorReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Aldebaran to Cataprom [ItemsByColorReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    var itemReferenceHomologado = _itemReferencesHomologadosService.GetById(item.ColorItemId);

                    _catapromItemByColorService.Remove(new DataAccess.Cataprom.ItemByColor { Id = itemReferenceHomologado.ReferenceIdHomologado });
                    _catapromItemByColorService.SaveChanges();
                    _aldebaranItemByColorService.Remove(item);
                    deleted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to delete an ItemByColor from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete an ItemByColor from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
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
