using System;
using System.Linq;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class UnitMeasuredSynchronize : IUnitMeasuredSynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.IUnitMeasuredService _aldebaranUnitMeasuredService;
        private readonly Domain.Cataprom.IUnitMeasuredService _catapromUnitMeasuredService;
        private readonly int _syncAttempts;

        public UnitMeasuredSynchronize(Domain.Aldebaran.IUnitMeasuredService aldebaranUnitMeasuredService, Domain.Cataprom.IUnitMeasuredService catapromUnitMeasuredService, IConfigurator configurator)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranUnitMeasuredService = aldebaranUnitMeasuredService;
            _catapromUnitMeasuredService = catapromUnitMeasuredService;
            _syncAttempts = configurator.GetKey("SyncAttempts").ToInt();
        }
        public void Sync()
        {
            var dataFirebird = _aldebaranUnitMeasuredService.Get(_syncAttempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)).ToList(); ;
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Firebird to Sql [UnitMeasuredSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Firebird to Sql [UnitMeasuredSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromUnitMeasuredService.AddOrUpdate(new DataAccess.Cataprom.UnitMeasured
                    {
                        Id = item.UnitMeasuredId,
                        Name = item.Name,
                        Active = "A"
                    });
                    _catapromUnitMeasuredService.SaveChanges();
                    _aldebaranUnitMeasuredService.Remove(item);
                    inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to insert/update a UnitMeasured from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to insert/update a UnitMeasured from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranUnitMeasuredService.Update(item);
                }
                finally
                {
                    _aldebaranUnitMeasuredService.SaveChanges();
                }
            }

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from UnitMeasured sql table");
        }

        public void ReverseSync()
        {
            var dataFirebird = _aldebaranUnitMeasuredService.Get(_syncAttempts).Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Firebird to Sql [UnitMeasuredReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Firebird to Sql [UnitMeasuredReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromUnitMeasuredService.Remove(new DataAccess.Cataprom.UnitMeasured { Id = item.UnitMeasuredId });
                    _catapromUnitMeasuredService.SaveChanges();
                    _aldebaranUnitMeasuredService.Remove(item);
                    deleted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to delete a UnitMeasured from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to delete a UnitMeasured from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranUnitMeasuredService.Update(item);
                }
                finally
                {
                    _aldebaranUnitMeasuredService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from UnitMeasured sql table");
        }
    }
}
