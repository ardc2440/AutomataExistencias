using System;
using System.Linq;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    public class LineSynchronize : ILineSynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.ILineService _aldebaranLineService;
        private readonly Domain.Cataprom.ILineService _catapromLineService;
        private readonly int _syncAttempts;
        public LineSynchronize(Domain.Aldebaran.ILineService aldebaranLineService, Domain.Cataprom.ILineService catapromLineService, IConfigurator configurator)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranLineService = aldebaranLineService;
            _catapromLineService = catapromLineService;
            _syncAttempts = configurator.GetKey("SyncAttempts").ToInt();
        }
        public void Sync()
        {
            var dataFirebird = _aldebaranLineService.Get(_syncAttempts).Where(w => string.Equals(w.Action, "I", StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Firebird to Sql [LinesSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Firebird to Sql [LinesSync]");

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
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to insert/update a Line from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to insert/update a Line from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
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
        public void ReverseSync()
        {
            var dataFirebird = _aldebaranLineService.Get(_syncAttempts).Where(w => string.Equals(w.Action, "D", StringComparison.CurrentCultureIgnoreCase)).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Firebird to Sql [LinesReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Firebird to Sql [LinesReverseSync]");

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
                    item.Exception = $"Attempts ({item.Attempts}/{_syncAttempts}): {ex.ToJson()}";
                    if (item.Attempts < _syncAttempts)
                        _logger.Error($"Internal error when trying to delete a Line from firebird to sql ({item.Attempts}/{_syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                    else
                        _logger.Fatal($"Exceeded attempts ({item.Attempts}/{_syncAttempts}) when trying to delete a Line from firebird to sql. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
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
