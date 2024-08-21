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
        private readonly Domain.Cataprom.IUnitMeasuredService _catapromUnitMeasuredService;
        private readonly Domain.Aldebaran.Homologacion.IMeasureUnitsHomologadosService _measureUnitsHomologadosService; 

        public UnitMeasuredSynchronize(Domain.Aldebaran.Homologacion.IMeasureUnitsHomologadosService measureUnitsHomologadosService, Domain.Aldebaran.IUnitMeasuredService aldebaranUnitMeasuredService, Domain.Cataprom.IUnitMeasuredService catapromUnitMeasuredService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranUnitMeasuredService = aldebaranUnitMeasuredService;
            _catapromUnitMeasuredService = catapromUnitMeasuredService;
            _measureUnitsHomologadosService = measureUnitsHomologadosService;
        }
        public void Sync(IEnumerable<UnitMeasured> data, int syncAttempts)
        {
            var dataFirebird = data.ToList(); ;
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [UnitMeasuredSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Aldebaran to Cataprom [UnitMeasuredSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    var measureUnitHomolgado = _measureUnitsHomologadosService.GetById(item.UnitMeasuredId);

                    _catapromUnitMeasuredService.AddOrUpdate(new DataAccess.Cataprom.UnitMeasured
                    {
                        Id = measureUnitHomolgado.MeasureUnitIdHomologado,
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
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to insert/update a UnitMeasured from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update a UnitMeasured from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
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

        public void ReverseSync(IEnumerable<UnitMeasured> data, int syncAttempts)
        {
            var dataFirebird = data.ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [UnitMeasuredReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Aldebaran to Cataprom [UnitMeasuredReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    var measureUnitHomolgado = _measureUnitsHomologadosService.GetById(item.UnitMeasuredId);

                    _catapromUnitMeasuredService.Remove(new DataAccess.Cataprom.UnitMeasured { Id = measureUnitHomolgado.MeasureUnitIdHomologado });
                    _catapromUnitMeasuredService.SaveChanges();
                    _aldebaranUnitMeasuredService.Remove(item);
                    deleted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to delete a UnitMeasured from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete a UnitMeasured from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
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
