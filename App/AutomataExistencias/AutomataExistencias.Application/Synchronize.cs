using System;
using System.Diagnostics;
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

        public Synchronize(Domain.Aldebaran.IStockService aldebaranStockService, Domain.Aldebaran.IItemService aldebaranItemService, Domain.Aldebaran.IItemByColorService aldebaranItemByColorService, Domain.Aldebaran.ILineService aldebaranLineService, Domain.Aldebaran.IMoneyService aldebaranMoneyService, Domain.Aldebaran.ITransitOrderService aldebaranTransitOrderService, Domain.Aldebaran.IUnitMeasuredService aldebaranUnitMeasuredService, Domain.Cataprom.IMoneyService catapromMoneyService)
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
            try
            {
                var dataFirebird = _aldebaranStockService.Get().ToList();
                if (!dataFirebird.Any())
                {
                    _logger.Info("No records to export from Firebird to Sql [StockSync]");
                    return;
                }
                _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [StockSync]");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToJson());
            }
        }
        public void ItemsSync()
        {
            try
            {
                var dataFirebird = _aldebaranItemService.Get().ToList();
                if (!dataFirebird.Any())
                {
                    _logger.Info("No records to export from Firebird to Sql [ItemsSync]");
                    return;
                }
                _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [ItemsSync]");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToJson());
            }
        }
        public void ItemsByColorSync()
        {
            try
            {
                var dataFirebird = _aldebaranItemByColorService.Get().ToList();
                if (!dataFirebird.Any())
                {
                    _logger.Info("No records to export from Firebird to Sql [ItemsByColorSync]");
                    return;
                }
                _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [ItemsByColorSync]");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToJson());
            }
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
                    _logger.Error($"Internal error when trying to update an money item from firebird to sql {ex.ToJson()}");
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