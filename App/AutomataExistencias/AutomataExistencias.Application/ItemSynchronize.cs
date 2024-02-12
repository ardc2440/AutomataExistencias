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
        private readonly Domain.Cataprom.IItemService _catapromItemService;
        public ItemSynchronize(Domain.Aldebaran.IItemService aldebaranItemService, Domain.Cataprom.IItemService catapromItemService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranItemService = aldebaranItemService;
            _catapromItemService = catapromItemService;
        }
        public void Sync(IEnumerable<Item> data, int syncAttempts)
        {
            var dataFirebird = data.ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [ItemsSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Aldebaran to Cataprom [ItemsSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromItemService.AddOrUpdate(new DataAccess.Cataprom.Item
                    {
                        Id = item.ItemId,
                        LineId = item.LineId.NullTo(),
                        Reference = item.Reference,
                        Name = item.Name,
                        ProviderReference = item.ProviderReference,
                        ProviderItemName = item.ProviderItemName,
                        ItemType = item.ItemType,
                        FobCost = item.FobCost.NullTo(),
                        MoneyId = item.MoneyId.NullTo(),
                        PartType = item.PartType,
                        Determinant = item.Determinant,
                        Observations = item.Observations,
                        StockExt = item.StockExt,
                        CifCost = item.CifCost,
                        Volume = item.Volume,
                        Weight = item.Weight,
                        FobUnitId = item.FobUnitId,
                        CifUnitId = item.CifUnitId,
                        NationalProduct = item.NationalProduct,
                        Active = item.Active,
                        VisibleCatalog = item.VisibleCatalog,
                    });
                    _catapromItemService.SaveChanges();
                    _aldebaranItemService.Remove(item);
                    inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to insert/update an Item from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update an Item from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranItemService.Update(item);
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
            var dataFirebird = data.ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [ItemsReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Aldebaran to Cataprom [ItemsReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromItemService.Remove(new DataAccess.Cataprom.Item { Id = item.ItemId });
                    _catapromItemService.SaveChanges();
                    _aldebaranItemService.Remove(item);
                    deleted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to delete an Item from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete an Item from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranItemService.Update(item);
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
