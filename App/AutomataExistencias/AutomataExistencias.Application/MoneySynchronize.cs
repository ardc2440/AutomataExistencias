using System;
using System.Linq;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class MoneySynchronize : IMoneySynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.IMoneyService _aldebaranMoneyService;
        private readonly Domain.Cataprom.IMoneyService _catapromMoneyService;
        private readonly int _syncAttempts;
        public MoneySynchronize(Domain.Aldebaran.IMoneyService aldebaranMoneyService, Domain.Cataprom.IMoneyService catapromMoneyService, IConfigurator configurator)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranMoneyService = aldebaranMoneyService;
            _catapromMoneyService = catapromMoneyService;
            _syncAttempts = configurator.GetKey("SyncAttempts").ToInt();
        }
        public void Sync()
        {
            var dataFirebird = _aldebaranMoneyService.Get(_syncAttempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Firebird to Sql [MoneySync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Firebird to Sql [MoneySync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromMoneyService.AddOrUpdate(new DataAccess.Cataprom.Money
                    {
                        Id = item.MoneyId,
                        Name = item.Name,
                        Active = "A"
                    });
                    _catapromMoneyService.SaveChanges();
                    _aldebaranMoneyService.Remove(item);
                    inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to insert/update a Money from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to insert/update a Money from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranMoneyService.Update(item);
                }
                finally
                {
                    _aldebaranMoneyService.SaveChanges();
                }
            }

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Money sql table");
        }
        public void ReverseSync()
        {
            var dataFirebird = _aldebaranMoneyService.Get(_syncAttempts).Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Firebird to Sql [MoneyReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Firebird to Sql [MoneyReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromMoneyService.Remove(new DataAccess.Cataprom.Money { Id = item.MoneyId });
                    _catapromMoneyService.SaveChanges();
                    _aldebaranMoneyService.Remove(item);
                    deleted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to delete a Money from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to delete a Money from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranMoneyService.Update(item);
                }
                finally
                {
                    _aldebaranMoneyService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Money sql table");
        }
    }
}