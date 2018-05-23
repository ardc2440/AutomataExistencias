using System;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.Domain.Aldebaran;
using NLog;

namespace AutomataExistencias.Application
{
    public class CleanerProcess : ICleanerProcess
    {
        private readonly ICleanService _cleanService;
        private readonly Logger _logger;
        public CleanerProcess(ICleanService cleanService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _cleanService = cleanService;
        }

        public void Clean(int daysToKeep)
        {
            try
            {
                _cleanService.Clean(daysToKeep);
            }
            catch (Exception ex)
            {
                _logger.Error($"Internal error when trying to clean firebird SyncTables {ex.ToJson()}");
            }
        }
    }
}