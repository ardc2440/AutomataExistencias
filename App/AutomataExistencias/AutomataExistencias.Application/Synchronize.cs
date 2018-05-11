using System;
using System.Diagnostics;
using System.Linq;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Cataprom;
using AutomataExistencias.Domain.Aldebaran;
using AutomataExistencias.Domain.Cataprom;
using NLog;

namespace AutomataExistencias.Application
{
    public class Synchronize : ISynchronize
    {
        private readonly IBalanceService _balanceService;
        private readonly Logger _logger;
        private readonly IStockItemService _stockItemService;

        public Synchronize(IBalanceService balanceService, IStockItemService stockItemService)
        {
            _balanceService = balanceService;
            _stockItemService = stockItemService;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Sync()
        {
            try
            {
                var dataFirebird = _balanceService.Get().ToList();
                if (!dataFirebird.Any())
                {
                    _logger.Info("No records to export from Firebird to Sql");
                    return;
                }
                _logger.Info($"Data retrieved from Firebird {dataFirebird.Count}");
                var stockList = dataFirebird.Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase))
                        .Select(s => new StockItem
                        {
                            ColorItemId = s.ColorItemId,
                            StorageCellar = s.StorageCellar
                        }).ToList();
                if (!stockList.Any())
                    _logger.Info($"Records to delete in Sql {stockList.Count}");
                _stockItemService.Remove(stockList);
                _stockItemService.SaveChanges();
                var c = 0;
                foreach (var item in dataFirebird.Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)))
                {
                    c++;
                    _stockItemService.AddOrUpdate(new StockItem
                    {
                        ColorItemId = item.ColorItemId,
                        ItemId = item.ItemId,
                        Color = item.Color,
                        Quantity = item.Quantity,
                        StorageCellar = item.StorageCellar
                    });
                }
                if (c > 0)
                    _logger.Info($"[{c}] records has been imported from Firebird to Sql");
                _stockItemService.SaveChanges();
                _balanceService.Remove(dataFirebird);
                _balanceService.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToJson());
            }
        }
    }
}