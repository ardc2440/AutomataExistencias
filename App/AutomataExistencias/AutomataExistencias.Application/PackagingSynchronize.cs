using System;
using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Aldebaran;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class PackagingSynchronize : IPackagingSynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.IPackagingService _aldebaranPackagingService;
        private readonly Domain.Cataprom.IPackagingService _catapromPackagingService;

        public PackagingSynchronize(Domain.Aldebaran.IPackagingService aldebaranPackagingService, Domain.Cataprom.IPackagingService catapromPackagingService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranPackagingService = aldebaranPackagingService;
            _catapromPackagingService = catapromPackagingService;
        }
        public void Sync(IEnumerable<Packaging> data, int syncAttempts)
        {
            var dataFirebird = data.ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [PackagingSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Aldebaran to Cataprom [PackagingSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
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
                    _catapromPackagingService.SaveChanges();
                    _aldebaranPackagingService.Remove(item);
                    inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to insert/update a Packaging from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update a Packaging from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranPackagingService.Update(item);
                }
                finally
                {
                    _aldebaranPackagingService.SaveChanges();
                }
            }

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Packaging sql table");
        }
        public void ReverseSync(IEnumerable<Packaging> data, int syncAttempts)
        {
            var dataFirebird = data.ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [PackagingReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Aldebaran to Cataprom [PackagingReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromPackagingService.Remove(new DataAccess.Cataprom.Packaging { Id = item.PackagingId });
                    _catapromPackagingService.SaveChanges();
                    _aldebaranPackagingService.Remove(item);
                    deleted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to delete a Packaging from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete a Packaging from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranPackagingService.Update(item);
                }
                finally
                {
                    _aldebaranPackagingService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Packaging sql table");
        }
    }
}
