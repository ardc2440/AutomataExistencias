using System;
using System.Linq;
using AutomataExistencias.Core.Extensions;
using NLog;

namespace AutomataExistencias.Application
{
    public class Synchronize : ISynchronize
    {
        private readonly Logger _logger;
        /*Aldebaran*/
        private readonly Domain.Aldebaran.IStockService _aldebaranStockService;
        private readonly Domain.Aldebaran.IItemService _aldebaranItemService;
        private readonly Domain.Aldebaran.IItemByColorService _aldebaranItemByColorService;
        private readonly Domain.Aldebaran.ILineService _aldebaranLineService;
        private readonly Domain.Aldebaran.IMoneyService _aldebaranMoneyService;
        private readonly Domain.Aldebaran.ITransitOrderService _aldebaranTransitOrderService;
        private readonly Domain.Aldebaran.IUnitMeasuredService _aldebaranUnitMeasuredService;

        /*Cataprom*/
        private readonly Domain.Cataprom.IMoneyService _catapromMoneyService;
        private readonly Domain.Cataprom.IItemService _catapromItemService;
        private readonly Domain.Cataprom.IItemByColorService _catapromItemByColorService;

        public Synchronize(Domain.Aldebaran.IStockService aldebaranStockService, Domain.Aldebaran.IItemService aldebaranItemService, Domain.Aldebaran.IItemByColorService aldebaranItemByColorService, Domain.Aldebaran.ILineService aldebaranLineService, Domain.Aldebaran.IMoneyService aldebaranMoneyService, Domain.Aldebaran.ITransitOrderService aldebaranTransitOrderService, Domain.Aldebaran.IUnitMeasuredService aldebaranUnitMeasuredService,
            Domain.Cataprom.IMoneyService catapromMoneyService, Domain.Cataprom.IItemService catapromItemService, Domain.Cataprom.IItemByColorService catapromItemByColorService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranStockService = aldebaranStockService;
            _aldebaranItemService = aldebaranItemService;
            _aldebaranItemByColorService = aldebaranItemByColorService;
            _aldebaranLineService = aldebaranLineService;
            _aldebaranMoneyService = aldebaranMoneyService;
            _aldebaranTransitOrderService = aldebaranTransitOrderService;
            _aldebaranUnitMeasuredService = aldebaranUnitMeasuredService;
            _catapromMoneyService = catapromMoneyService;

            _catapromItemService = catapromItemService;
            _catapromItemByColorService = catapromItemByColorService;
        }
        //public void Sync()
        //{
        //    try
        //    {
        //        var dataFirebird = _balanceService.Get().ToList();
        //        if (!dataFirebird.Any())
        //        {
        //            _logger.Info("No records to export from Firebird to Sql");
        //            return;
        //        }
        //        _logger.Info($"Data retrieved from Firebird {dataFirebird.Count}");
        //        var stockList = dataFirebird.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase))
        //                .Select(s => new StockItem
        //                {
        //                    ColorItemId = s.ColorItemId,
        //                    StorageCellar = s.StorageCellar
        //                }).ToList();
        //        if (!stockList.Any())
        //            _logger.Info($"Records to delete in Sql {stockList.Count}");
        //        _stockItemService.Remove(stockList);
        //        _stockItemService.SaveChanges();
        //        var c = 0;
        //        foreach (var item in dataFirebird.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)))
        //        {
        //            c++;
        //            _stockItemService.AddOrUpdate(new StockItem
        //            {
        //                ColorItemId = item.ColorItemId,
        //                ItemId = item.ItemId,
        //                Color = item.Color,
        //                Quantity = item.Quantity,
        //                StorageCellar = item.StorageCellar
        //            });
        //        }
        //        if (c > 0)
        //            _logger.Info($"[{c}] records has been imported from Firebird to Sql");
        //        _stockItemService.SaveChanges();
        //        _balanceService.Remove(dataFirebird);
        //        _balanceService.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex.ToJson());
        //    }
        //}
        public void StockSync()
        {

            var dataFirebird = _aldebaranStockService.Get().ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [StockSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [StockSync]");

        }
        public void ItemsSync()
        {
            var dataFirebird = _aldebaranItemService.Get().ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [ItemsSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [ItemsSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        deleted++;
                        _catapromItemService.Remove(new DataAccess.Cataprom.Item { Id = item.ItemId });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
                    {
                        inserted++;
                        _catapromItemService.AddOrUpdate(new DataAccess.Cataprom.Item
                        {
                            Id = item.MoneyId,
                            LineId = item.LineId,
                            Reference = item.Reference,
                            Name = item.Name,
                            ProviderReference = item.ProviderReference,
                            ProviderItemName = item.ProviderItemName,
                            ItemType = item.ItemType,
                            FobCost = item.FobCost,
                            MoneyId = item.MoneyId,
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
                    }
                    _catapromItemService.SaveChanges();
                    _aldebaranItemService.Remove(item);
                    _aldebaranItemService.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Internal error when trying to update an Item from firebird to sql {ex.ToJson()}");
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Item sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Item sql table");
        }
        public void ItemsByColorSync()
        {
            var dataFirebird = _aldebaranItemByColorService.Get().ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [ItemsByColorSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [ItemsByColorSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        deleted++;
                        _catapromItemByColorService.Remove(new DataAccess.Cataprom.ItemByColor { Id = item.ItemId });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
                    {
                        inserted++;
                        _catapromItemByColorService.AddOrUpdate(new DataAccess.Cataprom.ItemByColor
                        {
                            Id = item.ColorItemId,
                            ItemId = item.ItemId,
                            ItemByColorReference = item.ItemByColorReference,
                            ItemByColorInternalReference = item.ItemByColorInternalReference,
                            ColorName = item.ColorName,
                            ProviderNomItemByColor = item.ProviderNomItemByColor,
                            Observations = item.Observations,
                            Color = item.Color,
                            QuantityOrder = item.QuantityOrder,
                            Quantity = item.Quantity,
                            QuantityReserved = item.QuantityReserved,
                            QuantityOrderPan = item.QuantityOrderPan,
                            QuantityPan = item.QuantityPan,
                            QuantityReservedPan = item.QuantityReservedPan,
                            SoldOut = item.SoldOut,
                            QuantityProcess = item.QuantityProcess,
                        });
                    }
                    _catapromItemByColorService.SaveChanges();
                    _aldebaranItemByColorService.Remove(item);
                    _aldebaranItemByColorService.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Internal error when trying to update an ItemByColor from firebird to sql {ex.ToJson()}");
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from ItemByColor sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from ItemByColor sql table");
        }
        public void LinesSync()
        {
            try
            {
                var dataFirebird = _aldebaranLineService.Get().ToList();
                if (!dataFirebird.Any())
                {
                    _logger.Info("No records to export from Firebird to Sql [LinesSync]");
                    return;
                }
                _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [LinesSync]");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToJson());
            }
        }

        public void MoneySync()
        {
            var dataFirebird = _aldebaranMoneyService.Get().ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [MoneySync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [MoneySync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        deleted++;
                        _catapromMoneyService.Remove(new DataAccess.Cataprom.Money { Id = item.MoneyId });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
                    {
                        inserted++;
                        _catapromMoneyService.AddOrUpdate(new DataAccess.Cataprom.Money
                        {
                            Id = item.MoneyId,
                            Name = item.Name,
                            Active = "A"
                        });
                    }
                    _catapromMoneyService.SaveChanges();
                    _aldebaranMoneyService.Remove(item);
                    _aldebaranMoneyService.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Internal error when trying to update a Money from firebird to sql {ex.ToJson()}");
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Money sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Money sql table");
        }

        public void TransitOrderSync()
        {
            try
            {
                var dataFirebird = _aldebaranTransitOrderService.Get().ToList();
                if (!dataFirebird.Any())
                {
                    _logger.Info("No records to export from Firebird to Sql [TransitOrderSync]");
                    return;
                }
                _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [TransitOrderSync]");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToJson());
            }
        }

        public void UnitMeasuredSync()
        {
            try
            {
                var dataFirebird = _aldebaranUnitMeasuredService.Get().ToList();
                if (!dataFirebird.Any())
                {
                    _logger.Info("No records to export from Firebird to Sql [UnitMeasuredSync]");
                    return;
                }
                _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [LinesSync]");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToJson());
            }
        }
    }
}