using System;
using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.DataAccess.Aldebaran;
using Newtonsoft.Json;
using NLog;

namespace AutomataExistencias.Application
{
    // This class synchronizes lines between Aldebaran and Cataprom
    public class LineSynchronize : ILineSynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Aldebaran.ILineService _aldebaranLineService;
        private readonly ICatapromDestinationRunner _catapromDestinationRunner;
        private readonly Core.IAutomataState _automataState;
        private readonly IConnectivityErrorClassifier _connectivityErrorClassifier;
        public LineSynchronize(Domain.Aldebaran.ILineService aldebaranLineService, ICatapromDestinationRunner catapromDestinationRunner, Core.IAutomataState automataState, IConnectivityErrorClassifier connectivityErrorClassifier)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranLineService = aldebaranLineService;
            _catapromDestinationRunner = catapromDestinationRunner;
            _automataState = automataState;
            _connectivityErrorClassifier = connectivityErrorClassifier;
        }
        public void Sync(IEnumerable<Line> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(l => l.Id);
            var inserted = 0;
            var processed = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var catapromLineService = new Domain.Cataprom.LineService(unitOfWorkCataprom);
                        catapromLineService.AddOrUpdate(new DataAccess.Cataprom.Line
                        {
                            Id = item.LineId,
                            Code = item.Code,
                            Name = item.Name,
                            Daemon = item.Daemon,
                            Active = item.Active
                        });
                        catapromLineService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to insert/update a Line from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: Id={item.Id},LineId={item.LineId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update a Line from Aldebaran to Cataprom. | Data: Id={item.Id},LineId={item.LineId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                        try
                        {
                            if (_connectivityErrorClassifier.IsDestinationConnectivityError(item.Exception))
                                _automataState.IncrementConnectivityError("Line", connection.InventoryAutomationConnectionId);
                        }
                        catch { }
                    }
                });

                try
                {
                    processed++;
                    if (allDestinationsOk)
                    {
                        _aldebaranLineService.Remove(item);
                        inserted++;
                    }
                    else
                    {
                        _aldebaranLineService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranLineService.SaveChanges();
                }
            }
            if (processed == 0)
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [LinesSync]");
                return;
            }

            _logger.Info($"Found {processed} records to insert/update from Aldebaran to Cataprom [LinesSync]");

            if (inserted > 0)
                _logger.Info($"{inserted} records has been inserted/updated from Line sql table");
        }
        public void ReverseSync(IEnumerable<Line> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(l => l.Id);
            var deleted = 0;
            var processed = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var catapromLineService = new Domain.Cataprom.LineService(unitOfWorkCataprom);
                        catapromLineService.Remove(new DataAccess.Cataprom.Line { Id = item.LineId });
                        catapromLineService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to delete a Line from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: Id={item.Id},LineId={item.LineId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete a Line from Aldebaran to Cataprom. | Data: Id={item.Id},LineId={item.LineId},Attempts={item.Attempts} | Exception: {ex.ToJson()}");
                    }
                });

                try
                {
                    if (allDestinationsOk)
                    {
                        _aldebaranLineService.Remove(item);
                        deleted++;
                    }
                    else
                    {
                        _aldebaranLineService.Update(item);
                    }
                }
                finally
                {
                    _aldebaranLineService.SaveChanges();
                }
                processed++;
            }
            if (processed == 0)
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [LinesReverseSync]");
                return;
            }

            _logger.Info($"Found {processed} records to delete from Aldebaran to Cataprom [LinesReverseSync]");

            if (deleted > 0)
                _logger.Info($"{deleted} records has been deleted from Line sql table");
        }
    }
}
