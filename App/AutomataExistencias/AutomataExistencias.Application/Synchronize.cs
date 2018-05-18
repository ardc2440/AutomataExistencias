using System;
using System.Linq;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using Newtonsoft.Json;
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
        private readonly Domain.Aldebaran.IPackagingService _aldebaranPackagingService;

        /*Cataprom*/
        private readonly Domain.Cataprom.IStockService _catapromStockService;
        private readonly Domain.Cataprom.IMoneyService _catapromMoneyService;
        private readonly Domain.Cataprom.IItemService _catapromItemService;
        private readonly Domain.Cataprom.IItemByColorService _catapromItemByColorService;
        private readonly Domain.Cataprom.ILineService _catapromLineService;
        private readonly Domain.Cataprom.IUnitMeasuredService _catapromUnitMeasuredService;
        private readonly Domain.Cataprom.ITransitOrderService _catapromTransitOrderService;
        private readonly Domain.Cataprom.IUpdateProcessService _catapromUpdateProcessService;
        private readonly Domain.Cataprom.IPackagingService _catapromPackagingService;
        private readonly int _syncAttempts;
        public Synchronize(Domain.Aldebaran.IStockService aldebaranStockService, Domain.Aldebaran.IItemService aldebaranItemService, Domain.Aldebaran.IItemByColorService aldebaranItemByColorService, Domain.Aldebaran.ILineService aldebaranLineService, Domain.Aldebaran.IMoneyService aldebaranMoneyService, Domain.Aldebaran.ITransitOrderService aldebaranTransitOrderService, Domain.Aldebaran.IUnitMeasuredService aldebaranUnitMeasuredService, Domain.Aldebaran.IPackagingService aldebaranPackagingService,
            Domain.Cataprom.IMoneyService catapromMoneyService, Domain.Cataprom.IItemService catapromItemService, Domain.Cataprom.IItemByColorService catapromItemByColorService, Domain.Cataprom.ILineService catapromLineService, Domain.Cataprom.IUnitMeasuredService catapromUnitMeasuredService, Domain.Cataprom.ITransitOrderService catapromTransitOrderService, Domain.Cataprom.IStockService catapromStockService, Domain.Cataprom.IUpdateProcessService catapromUpdateProcessService, Domain.Cataprom.IPackagingService catapromPackagingService,
            IConfigurator configurator)
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
            _aldebaranPackagingService = aldebaranPackagingService;
            /*Cataprom*/
            _catapromItemService = catapromItemService;
            _catapromItemByColorService = catapromItemByColorService;
            _catapromLineService = catapromLineService;
            _catapromUnitMeasuredService = catapromUnitMeasuredService;
            _catapromTransitOrderService = catapromTransitOrderService;
            _catapromStockService = catapromStockService;
            _catapromMoneyService = catapromMoneyService;
            _catapromUpdateProcessService = catapromUpdateProcessService;
            _catapromPackagingService = catapromPackagingService;
            _syncAttempts = configurator.GetKey("SyncAttempts").ToInt();
        }
        public void StockSync()
        {
            var dataFirebird = _aldebaranStockService.Get(_syncAttempts).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [StockSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to export from Firebird to Sql [StockSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _catapromStockService.Remove(new DataAccess.Cataprom.Stock
                        {
                            ColorItemId = item.ColorItemId,
                            StorageCellar = item.StorageCellar
                        });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
                    {
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
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase)) deleted++;
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase)) inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to update a Stock from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to update a Stock from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranStockService.Update(item);
                }
                finally
                {
                    _aldebaranStockService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Stock sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Stock sql table");
        }
        public void ItemsSync()
        {
            var dataFirebird = _aldebaranItemService.Get(_syncAttempts).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [ItemsSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to export from Firebird to Sql [ItemsSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _catapromItemService.Remove(new DataAccess.Cataprom.Item { Id = item.ItemId });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
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
                    }
                    _catapromItemService.SaveChanges();
                    _aldebaranItemService.Remove(item);
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase)) deleted++;
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase)) inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to update an Item from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to update an Item from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranItemService.Update(item);
                }
                finally
                {
                    _aldebaranItemService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Item sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Item sql table");
        }
        public void ItemsByColorSync()
        {
            var dataFirebird = _aldebaranItemByColorService.Get(_syncAttempts).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [ItemsByColorSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to export from Firebird to Sql [ItemsByColorSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _catapromItemByColorService.Remove(new DataAccess.Cataprom.ItemByColor { Id = item.ColorItemId });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
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
                    }
                    _catapromItemByColorService.SaveChanges();
                    _aldebaranItemByColorService.Remove(item);
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase)) deleted++;
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase)) inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to update an ItemByColor from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to update an ItemByColor from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranItemByColorService.Update(item);
                }
                finally
                {
                    _aldebaranItemByColorService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from ItemByColor sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from ItemByColor sql table");
        }
        public void LinesSync()
        {
            var dataFirebird = _aldebaranLineService.Get(_syncAttempts).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [LinesSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to export from Firebird to Sql [LinesSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _catapromLineService.Remove(new DataAccess.Cataprom.Line { Id = item.LineId });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
                    {
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
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase)) deleted++;
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase)) inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to update a Line from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to update a Line from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranLineService.Update(item);
                }
                finally
                {
                    _aldebaranLineService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Line sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Line sql table");
        }
        public void MoneySync()
        {
            var dataFirebird = _aldebaranMoneyService.Get(_syncAttempts).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [MoneySync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to export from Firebird to Sql [MoneySync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _catapromMoneyService.Remove(new DataAccess.Cataprom.Money { Id = item.MoneyId });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _catapromMoneyService.AddOrUpdate(new DataAccess.Cataprom.Money
                        {
                            Id = item.MoneyId,
                            Name = item.Name,
                            Active = "A"
                        });
                    }
                    _catapromMoneyService.SaveChanges();
                    _aldebaranMoneyService.Remove(item);
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase)) deleted++;
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase)) inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to update a Money from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to update a Money from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranMoneyService.Update(item);
                }
                finally
                {
                    _aldebaranMoneyService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Money sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Money sql table");
        }
        public void TransitOrderSync()
        {
            var dataFirebird = _aldebaranTransitOrderService.Get(_syncAttempts).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [UnitMeasuredSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to export from Firebird to Sql [UnitMeasuredSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _catapromTransitOrderService.Remove(new DataAccess.Cataprom.TransitOrder
                        {
                            Id = item.TransitOrderItemId
                        });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
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
                    }
                    _catapromTransitOrderService.SaveChanges();
                    _aldebaranTransitOrderService.Remove(item);
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase)) deleted++;
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase)) inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to update a TransitOrder from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to update a TransitOrder from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranTransitOrderService.Update(item);
                }
                finally
                {
                    _aldebaranTransitOrderService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from TransitOrder sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from TransitOrder sql table");
        }
        public void UnitMeasuredSync()
        {
            var dataFirebird = _aldebaranUnitMeasuredService.Get(_syncAttempts).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [UnitMeasuredSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to export from Firebird to Sql [UnitMeasuredSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _catapromUnitMeasuredService.Remove(new DataAccess.Cataprom.UnitMeasured
                        {
                            Id = item.UnitMeasuredId
                        });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _catapromUnitMeasuredService.AddOrUpdate(new DataAccess.Cataprom.UnitMeasured
                        {
                            Id = item.UnitMeasuredId,
                            Name = item.Name,
                            Active = "A"
                        });
                    }
                    _catapromUnitMeasuredService.SaveChanges();
                    _aldebaranUnitMeasuredService.Remove(item);
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase)) deleted++;
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase)) inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to update a UnitMeasured from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to update a UnitMeasured from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranUnitMeasuredService.Update(item);
                }
                finally
                {
                    _aldebaranUnitMeasuredService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from UnitMeasured sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from UnitMeasured sql table");
        }
        public void PackagingSync()
        {
            var dataFirebird = _aldebaranPackagingService.Get(_syncAttempts).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to export from Firebird to Sql [PackagingSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to export from Firebird to Sql [PackagingSync]");

            var deleted = 0;
            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _catapromPackagingService.Remove(new DataAccess.Cataprom.Packaging { Id = item.PackagingId });
                    }
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _catapromPackagingService.AddOrUpdate(new DataAccess.Cataprom.Packaging
                        {
                            Id = item.PackagingId,
                            ItemId = item.ItemId,
                            Weight = item.Weight,
                            Height = item.Height,
                            Width = item.Width,
                            Long = item.Long,
                            Quantity = item.Quantity
                        });
                    }
                    _catapromPackagingService.SaveChanges();
                    _aldebaranPackagingService.Remove(item);
                    if (string.Equals(item.Action, "D", StringComparison.CurrentCultureIgnoreCase)) deleted++;
                    if (string.Equals(item.Action, "I", StringComparison.CurrentCultureIgnoreCase)) inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to update a Packaging from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to update a Packaging from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranPackagingService.Update(item);
                }
                finally
                {
                    _aldebaranPackagingService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Packaging sql table");
            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Packaging sql table");
        }
        public void UpdateProcess()
        {
            try
            {
                _catapromUpdateProcessService.Update();
            }
            catch (Exception ex)
            {
                _logger.Error($"Internal error when trying to update synk process in sql {ex.ToJson()}");
            }
        }
    }
}