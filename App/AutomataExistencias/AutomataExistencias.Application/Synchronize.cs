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
        private readonly Domain.Cataprom.IStockService _catapromStockService;
        private readonly Domain.Cataprom.IMoneyService _catapromMoneyService;
        private readonly Domain.Cataprom.IItemService _catapromItemService;
        private readonly Domain.Cataprom.IItemByColorService _catapromItemByColorService;
        private readonly Domain.Cataprom.ILineService _catapromLineService;
        private readonly Domain.Cataprom.IUnitMeasuredService _catapromUnitMeasuredService;
        private readonly Domain.Cataprom.ITransitOrderService _catapromTransitOrderService;

        public Synchronize(Domain.Aldebaran.IStockService aldebaranStockService, Domain.Aldebaran.IItemService aldebaranItemService, Domain.Aldebaran.IItemByColorService aldebaranItemByColorService, Domain.Aldebaran.ILineService aldebaranLineService, Domain.Aldebaran.IMoneyService aldebaranMoneyService, Domain.Aldebaran.ITransitOrderService aldebaranTransitOrderService, Domain.Aldebaran.IUnitMeasuredService aldebaranUnitMeasuredService,
            Domain.Cataprom.IMoneyService catapromMoneyService, Domain.Cataprom.IItemService catapromItemService, Domain.Cataprom.IItemByColorService catapromItemByColorService, Domain.Cataprom.ILineService catapromLineService, Domain.Cataprom.IUnitMeasuredService catapromUnitMeasuredService, Domain.Cataprom.ITransitOrderService catapromTransitOrderService, Domain.Cataprom.IStockService catapromStockService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            
            /*Aldebaran*/
            _aldebaranStockService = aldebaranStockService;
            _aldebaranItemService = aldebaranItemService;
            _aldebaranItemByColorService = aldebaranItemByColorService;
            _aldebaranLineService = aldebaranLineService;
            _aldebaranMoneyService = aldebaranMoneyService;
            _aldebaranTransitOrderService = aldebaranTransitOrderService;
            _aldebaranUnitMeasuredService = aldebaranUnitMeasuredService;
            
            /*Cataprom*/
            _catapromItemService = catapromItemService;
            _catapromItemByColorService = catapromItemByColorService;
            _catapromLineService = catapromLineService;
            _catapromUnitMeasuredService = catapromUnitMeasuredService;
            _catapromTransitOrderService = catapromTransitOrderService;
            _catapromStockService = catapromStockService;
            _catapromMoneyService = catapromMoneyService;
        }
        public void StockSync()
        {
            var dataFirebird = _aldebaranStockService.Get().ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [StockSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [StockSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        deleted++;
                        _catapromStockService.Remove(new DataAccess.Cataprom.Stock { ColorItemId = item.ColorItemId, StorageCellar= item.StorageCellar });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
                    {
                        inserted++;
                        _catapromStockService.AddOrUpdate(new DataAccess.Cataprom.Stock
                        {
                            ColorItemId = item.ColorItemId,
                            ItemId = item.ItemId,
                            Color = item.Color,
                            Quantity = item.Quantity,
                            StorageCellar = item.StorageCellar
                        });
                    }
                    _catapromStockService.SaveChanges();
                    _aldebaranStockService.Remove(item);
                    _aldebaranStockService.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Internal error when trying to update a Stock from firebird to sql {ex.ToJson()}");
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Stock sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Stock sql table");
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
            var dataFirebird = _aldebaranLineService.Get().ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [LinesSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [LinesSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        deleted++;
                        _catapromLineService.Remove(new DataAccess.Cataprom.Line { Id = item.LineId });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
                    {
                        inserted++;
                        _catapromLineService.AddOrUpdate(new DataAccess.Cataprom.Line
                        {
                            Id = item.LineId,
                            Code = item.Code,
                            Name = item.Name,
                            Daemon = item.Daemon,
                            Active = item.Active
                        });
                    }
                    _catapromLineService.SaveChanges();
                    _aldebaranLineService.Remove(item);
                    _aldebaranLineService.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Internal error when trying to update a Line from firebird to sql {ex.ToJson()}");
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Line sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Line sql table");
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
            var dataFirebird = _aldebaranTransitOrderService.Get().ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [UnitMeasuredSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [UnitMeasuredSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        deleted++;
                        _catapromTransitOrderService.Remove(new DataAccess.Cataprom.TransitOrder { Id = item.TransitOrderItemId });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
                    {
                        inserted++;
                        _catapromTransitOrderService.AddOrUpdate(new DataAccess.Cataprom.TransitOrder
                        {
                            Id = item.TransitOrderItemId,
                            DeliveredDate = item.DeliveredDate,
                            DeliveredQuantity = item.DeliveredQuantity,
                            Date = item.Date,
                            Activity = item.Activity,
                            ColorItemId = item.ColorItemId,
                        });
                    }
                    _catapromTransitOrderService.SaveChanges();
                    _aldebaranTransitOrderService.Remove(item);
                    _aldebaranTransitOrderService.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Internal error when trying to update a TransitOrder from firebird to sql {ex.ToJson()}");
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from TransitOrder sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from TransitOrder sql table");
        }
        public void UnitMeasuredSync()
        {
            var dataFirebird = _aldebaranUnitMeasuredService.Get().ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [UnitMeasuredSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} to export from Firebird to Sql [UnitMeasuredSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        deleted++;
                        _catapromUnitMeasuredService.Remove(new DataAccess.Cataprom.UnitMeasured { Id = item.UnitMeasuredId });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
                    {
                        inserted++;
                        _catapromUnitMeasuredService.AddOrUpdate(new DataAccess.Cataprom.UnitMeasured
                        {
                            Id = item.UnitMeasuredId,
                            Name = item.Name,
                            Active = "A"
                        });
                    }
                    _catapromUnitMeasuredService.SaveChanges();
                    _aldebaranUnitMeasuredService.Remove(item);
                    _aldebaranUnitMeasuredService.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Internal error when trying to update an UnitMeasured from firebird to sql {ex.ToJson()}");
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from UnitMeasured sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from UnitMeasured sql table");
        }
    }
}