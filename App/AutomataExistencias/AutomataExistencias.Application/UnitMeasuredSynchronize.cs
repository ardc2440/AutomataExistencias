using System;
using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Aldebaran;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class UnitMeasuredSynchronize : IUnitMeasuredSynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.IUnitMeasuredService _aldebaranUnitMeasuredService;
        private readonly ICatapromDestinationRunner _catapromDestinationRunner;
        private readonly Domain.Aldebaran.Homologacion.IMeasureUnitsHomologadosService _measureUnitsHomologadosService; 
        private readonly Core.IAutomataState _automataState;
        private readonly IConnectivityErrorClassifier _connectivityErrorClassifier;

        public UnitMeasuredSynchronize(Domain.Aldebaran.Homologacion.IMeasureUnitsHomologadosService measureUnitsHomologadosService, Domain.Aldebaran.IUnitMeasuredService aldebaranUnitMeasuredService, ICatapromDestinationRunner catapromDestinationRunner, Core.IAutomataState automataState, IConnectivityErrorClassifier connectivityErrorClassifier)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranUnitMeasuredService = aldebaranUnitMeasuredService;
            _catapromDestinationRunner = catapromDestinationRunner;
            _measureUnitsHomologadosService = measureUnitsHomologadosService;
            _automataState = automataState;
            _connectivityErrorClassifier = connectivityErrorClassifier;
        }
        public void Sync(IEnumerable<UnitMeasured> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(u => u.Id);
            var inserted = 0;
            var processed = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var measureUnitHomolgado = _measureUnitsHomologadosService.GetById(item.UnitMeasuredId);

                        var catapromUnitMeasuredService = new Domain.Cataprom.UnitMeasuredService(unitOfWorkCataprom);
                        catapromUnitMeasuredService.AddOrUpdate(new DataAccess.Cataprom.UnitMeasured
                        {
                            Id = measureUnitHomolgado.MeasureUnitIdHomologado,
                            Name = item.Name,
                            Active = "A"
                        });
                        catapromUnitMeasuredService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to insert/update a UnitMeasured from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: Id={item.Id},UnitMeasuredId={item.UnitMeasuredId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update a UnitMeasured from Aldebaran to Cataprom. | Data: Id={item.Id},UnitMeasuredId={item.UnitMeasuredId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");

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
                        _aldebaranUnitMeasuredService.Remove(item);
                        inserted++;
                    }
                    else
                    {
                        _aldebaranUnitMeasuredService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranUnitMeasuredService.SaveChanges();
                }
            }
            if (processed == 0)
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [UnitMeasuredSync]");
                return;
            }

            _logger.Info($"Found {processed} records to insert/update from Aldebaran to Cataprom [UnitMeasuredSync]");

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from UnitMeasured sql table");
        }

        public void ReverseSync(IEnumerable<UnitMeasured> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(u => u.Id);
            var deleted = 0;
            var processed = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var measureUnitHomolgado = _measureUnitsHomologadosService.GetById(item.UnitMeasuredId);

                        var catapromUnitMeasuredService = new Domain.Cataprom.UnitMeasuredService(unitOfWorkCataprom);
                        catapromUnitMeasuredService.Remove(new DataAccess.Cataprom.UnitMeasured { Id = measureUnitHomolgado.MeasureUnitIdHomologado });
                        catapromUnitMeasuredService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to delete a UnitMeasured from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: Id={item.Id},UnitMeasuredId={item.UnitMeasuredId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete a UnitMeasured from Aldebaran to Cataprom. | Data: Id={item.Id},UnitMeasuredId={item.UnitMeasuredId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                    }
                });

                try
                {
                    if (allDestinationsOk)
                    {
                        _aldebaranUnitMeasuredService.Remove(item);
                        deleted++;
                    }
                    else
                    {
                        _aldebaranUnitMeasuredService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranUnitMeasuredService.SaveChanges();
                }
                processed++;
            }
            if (processed == 0)
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [UnitMeasuredReverseSync]");
                return;
            }

            _logger.Info($"Found {processed} records to delete from Aldebaran to Cataprom [UnitMeasuredReverseSync]");

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from UnitMeasured sql table");
        }
    }
}
