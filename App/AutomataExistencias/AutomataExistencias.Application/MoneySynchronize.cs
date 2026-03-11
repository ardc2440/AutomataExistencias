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
        private readonly ICatapromDestinationRunner _catapromDestinationRunner;
        private readonly Core.IAutomataState _automataState;
        private readonly IConnectivityErrorClassifier _connectivityErrorClassifier;
        private readonly Domain.Aldebaran.Homologacion.ICurrenciesHomologadosService _currenciesHomologadosService;

        public MoneySynchronize(Domain.Aldebaran.Homologacion.ICurrenciesHomologadosService currenciesHomologadosService, Domain.Aldebaran.IMoneyService aldebaranMoneyService, ICatapromDestinationRunner catapromDestinationRunner, Core.IAutomataState automataState, IConnectivityErrorClassifier connectivityErrorClassifier)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranMoneyService = aldebaranMoneyService;
            _catapromDestinationRunner = catapromDestinationRunner;
            _automataState = automataState;
            _connectivityErrorClassifier = connectivityErrorClassifier;
            _currenciesHomologadosService = currenciesHomologadosService;
        }
        public void Sync(IEnumerable<Money> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(m => m.Id);
            var inserted = 0;
            var processed = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var currencyHomologado = _currenciesHomologadosService.GetById(item.MoneyId);

                        var catapromMoneyService = new Domain.Cataprom.MoneyService(unitOfWorkCataprom);
                        catapromMoneyService.AddOrUpdate(new DataAccess.Cataprom.Money
                        {
                            Id = currencyHomologado.CurrencyIdHomologado,
                            Name = item.Name,
                            Active = "A"
                        });
                        catapromMoneyService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to insert/update a Money from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: Id={item.Id},MoneyId={item.MoneyId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                        else
                        {
                            var full = SerializationThrottler.SerializeIfAllowed(item, "MoneySync");
                            if (!string.IsNullOrEmpty(full))
                                _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update a Money from Aldebaran to Cataprom. | FullData: {full} | Exception: {ex.ToJson()}");
                            else
                                _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update a Money from Aldebaran to Cataprom. | Data: Id={item.Id},MoneyId={item.MoneyId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                        }

                        try
                        {
                            var exText = ex.ToString();
                            var isConn = _connectivityErrorClassifier.IsDestinationConnectivityError(exText);
                            _automataState.RecordAttempt(connection.InventoryAutomationConnectionId, isConn);
                        }
                        catch { }
                    }
                });

                try
                {
                    processed++;
                    if (allDestinationsOk)
                    {
                        _aldebaranMoneyService.Remove(item);
                        inserted++;
                    }
                    else
                    {
                        _aldebaranMoneyService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranMoneyService.SaveChanges();
                }
            }
            if (processed == 0)
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [MoneySync]");
                return;
            }

            _logger.Info($"Found {processed} records to insert/update from Aldebaran to Cataprom [MoneySync]");

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Money sql table");
        }
        public void ReverseSync(IEnumerable<Money> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(m => m.Id);
            var deleted = 0;
            var processed = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var currencyHomologado = _currenciesHomologadosService.GetById(item.MoneyId);

                        if (currencyHomologado != null)
                        {
                            var catapromMoneyService = new Domain.Cataprom.MoneyService(unitOfWorkCataprom);
                            catapromMoneyService.Remove(new DataAccess.Cataprom.Money { Id = currencyHomologado.CurrencyIdHomologado });
                            catapromMoneyService.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to delete a Money from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: Id={item.Id},MoneyId={item.MoneyId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete a Money from Aldebaran to Cataprom. | Data: Id={item.Id},MoneyId={item.MoneyId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                    }
                });

                try
                {
                    if (allDestinationsOk)
                    {
                        _aldebaranMoneyService.Remove(item);
                        deleted++;
                    }
                    else
                    {
                        _aldebaranMoneyService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranMoneyService.SaveChanges();
                }
                processed++;
            }
            if (processed == 0)
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [MoneyReverseSync]");
                return;
            }

            _logger.Info($"Found {processed} records to delete from Aldebaran to Cataprom [MoneyReverseSync]");

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Money sql table");
        }
    }
}