using System;
using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Aldebaran;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class StockSynchronize : IStockSynchronize
    {
        private readonly Logger _logger;
        /*Aldebaran*/
        private readonly Domain.Aldebaran.IStockService _aldebaranStockService;
        private readonly Domain.Cataprom.IStockService _catapromStockService;
        public StockSynchronize(Domain.Aldebaran.IStockService aldebaranStockService, Domain.Cataprom.IStockService catapromStockService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranStockService = aldebaranStockService;
            _catapromStockService = catapromStockService;
        }
        public void Sync(IEnumerable<Stock> data, int syncAttempts)
        {
            var dataFirebird = data.ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Firebird to Sql [StockSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Firebird to Sql [StockSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromStockService.AddOrUpdate(new DataAccess.Cataprom.Stock
                    {
                        ColorItemId = item.ColorItemId,
                        ItemId = item.ItemId,
                        Color = item.Color,
                        Quantity = item.Quantity,
                        StorageCellar = item.StorageCellar
                    });
                    _catapromStockService.SaveChanges();
                    _aldebaranStockService.Remove(item);
                    inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to insert/update a Stock from firebird to sql ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update a Stock from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranStockService.Update(item);
                }
                finally
                {
                    _aldebaranStockService.SaveChanges();
                }
            }

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Stock sql table");
        }
        public void ReverseSync(IEnumerable<Stock> data, int syncAttempts)
        {
            var dataFirebird = data.ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Firebird to Sql [StockReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Firebird to Sql [StockReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromStockService.Remove(new DataAccess.Cataprom.Stock
                    {
                        ColorItemId = item.ColorItemId,
                        StorageCellar = item.StorageCellar
                    });
                    _catapromStockService.SaveChanges();
                    _aldebaranStockService.Remove(item);
                    deleted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to delete a Stock from firebird to sql ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete a Stock from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranStockService.Update(item);
                }
                finally
                {
                    _aldebaranStockService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Stock sql table");
        }
    }
}
