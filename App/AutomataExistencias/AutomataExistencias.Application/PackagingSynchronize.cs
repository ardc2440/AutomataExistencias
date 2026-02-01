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
        private readonly ICatapromDestinationRunner _catapromDestinationRunner;
        private readonly Domain.Aldebaran.Homologacion.IPackagingHomologadosService _packagingHomologadosService;
        private readonly Core.IAutomataState _automataState;
        private readonly IConnectivityErrorClassifier _connectivityErrorClassifier;

        public PackagingSynchronize(Domain.Aldebaran.Homologacion.IPackagingHomologadosService packagingHomologadosService, Domain.Aldebaran.IPackagingService aldebaranPackagingService, ICatapromDestinationRunner catapromDestinationRunner, Core.IAutomataState automataState, IConnectivityErrorClassifier connectivityErrorClassifier)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _aldebaranPackagingService = aldebaranPackagingService;
            _catapromDestinationRunner = catapromDestinationRunner;
            _packagingHomologadosService = packagingHomologadosService;
            _automataState = automataState;
            _connectivityErrorClassifier = connectivityErrorClassifier;
        }
        public void Sync(IEnumerable<Packaging> data, int syncAttempts)
        {
            var dataFirebird = data.OrderBy(p => p.Id).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to insert/update from Aldebaran to Cataprom [PackagingSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to insert/update from Aldebaran to Cataprom [PackagingSync]");

            var inserted = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var packagingHomologado = _packagingHomologadosService.GetById(item.PackagingId);

                        var catapromPackagingService = new Domain.Cataprom.PackagingService(unitOfWorkCataprom);
                        catapromPackagingService.AddOrUpdate(new DataAccess.Cataprom.Packaging
                        {
                            Id = packagingHomologado.PackagingIdHomologado,
                            ItemId = packagingHomologado.ItemIdHomologado,
                            Weight = (decimal)item.Weight,
                            Height = (decimal)item.Height,
                            Width = (decimal)item.Width,
                            Long = (decimal)item.Long,
                            Quantity = item.Quantity
                        });
                        catapromPackagingService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to insert/update a Packaging from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to insert/update a Packaging from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
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
                    if (allDestinationsOk)
                    {
                        _aldebaranPackagingService.Remove(item);
                        inserted++;
                    }
                    else
                    {
                        _aldebaranPackagingService.Update(item);
                    }
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
            var dataFirebird = data.OrderBy(p => p.Id).ToList();
            if (!dataFirebird.Any())
            {
                _logger.Info("No records to delete from Aldebaran to Cataprom [PackagingReverseSync]");
                return;
            }
            _logger.Info($"Found {dataFirebird.Count} records to delete from Aldebaran to Cataprom [PackagingReverseSync]");

            var deleted = 0;
            foreach (var item in dataFirebird)
            {
                var allDestinationsOk = true;

                _catapromDestinationRunner.RunForAllDestinations((unitOfWorkCataprom, connection) =>
                {
                    try
                    {
                        var packagingHomologado = _packagingHomologadosService.GetById(item.PackagingId);

                        var catapromPackagingService = new Domain.Cataprom.PackagingService(unitOfWorkCataprom);
                        catapromPackagingService.Remove(new DataAccess.Cataprom.Packaging { Id = packagingHomologado.PackagingIdHomologado });
                        catapromPackagingService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        allDestinationsOk = false;
                        item.Attempts++;
                        var connInfo = $"ConnId={connection.InventoryAutomationConnectionId}, Server={connection.ServerName}, Database={connection.DatabaseName}";
                        item.Exception = $"{connInfo} | Attempts ({item.Attempts}/{syncAttempts}): {ex.ToJson()}";
                        if (item.Attempts < syncAttempts)
                            _logger.Error($"[{connInfo}] Internal error when trying to delete a Packaging from Aldebaran to Cataprom ({item.Attempts}/{syncAttempts}) | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");
                        else
                            _logger.Fatal($"[{connInfo}] Exceeded attempts ({item.Attempts}/{syncAttempts}) when trying to delete a Packaging from Aldebaran to Cataprom. | Data: {JsonConvert.SerializeObject(item)} | Exception: {ex.ToJson()}");

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
                    if (allDestinationsOk)
                    {
                        _aldebaranPackagingService.Remove(item);
                        deleted++;
                    }
                    else
                    {
                        _aldebaranPackagingService.Update(item);
                    }
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
