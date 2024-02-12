using System;
using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Aldebaran;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class LineSynchronize : ILineSynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.ILineService _aldebaranLineService;
        private readonly Domain.Cataprom.ILineService _catapromLineService;
        public LineSynchronize(Domain.Aldebaran.ILineService aldebaranLineService, Domain.Cataprom.ILineService catapromLineService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranLineService = aldebaranLineService;
            _catapromLineService = catapromLineService;
        }
        public void Sync(IEnumerable<Line> data, int syncAttempts)
        {
            var dataFirebird = data.ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [LinesSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Aldebaran to Cataprom [LinesSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromLineService.AddOrUpdate(new DataAccess.Cataprom.Line
                    {
                        Id = item.LineId,
                        Code = item.Code,
                        Name = item.Name,
                        Daemon = item.Daemon,
                        Active = item.Active
                    });
                    _catapromLineService.SaveChanges();
                    _aldebaranLineService.Remove(item);
                    inserted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to insert/update a Line from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update a Line from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranLineService.Update(item);
                }
                finally
                {
                    _aldebaranLineService.SaveChanges();
                }
            }

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Line sql table");
        }
        public void ReverseSync(IEnumerable<Line> data, int syncAttempts)
        {
            var dataFirebird = data.ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [LinesReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Aldebaran to Cataprom [LinesReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                try
                {
                    _catapromLineService.Remove(new DataAccess.Cataprom.Line { Id = item.LineId });
                    _catapromLineService.SaveChanges();
                    _aldebaranLineService.Remove(item);
                    deleted++;
                }
                catch (Exception ex)
                {
                    item.Attempts++;
                    item.Exception = $"Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < syncAttempts)
                        _logger.Error($"Internal error when trying to delete a Line from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete a Line from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    _aldebaranLineService.Update(item);
                }
                finally
                {
                    _aldebaranLineService.SaveChanges();
                }
            }

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Line sql table");
        }
    }
}
