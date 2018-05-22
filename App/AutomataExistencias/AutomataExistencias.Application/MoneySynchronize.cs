using System;
using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Aldebaran;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class MoneySynchronize : IMoneySynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.IMoneyService _aldebaranMoneyService;
        private readonly Domain.Cataprom.IMoneyService _catapromMoneyService;
        public MoneySynchronize(Domain.Aldebaran.IMoneyService aldebaranMoneyService, Domain.Cataprom.IMoneyService catapromMoneyService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranMoneyService = aldebaranMoneyService;
            _catapromMoneyService = catapromMoneyService;
        }
        public void Sync(IEnumerable<Money> data, int syncAttempts)
        {
            var dataFirebird = data.ToList();
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
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to insert/update a Money from firebird to sql ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update a Money from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
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
        public void ReverseSync(IEnumerable<Money> data, int syncAttempts)
        {
            var dataFirebird = data.ToList();
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
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to delete a Money from firebird to sql ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete a Money from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
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