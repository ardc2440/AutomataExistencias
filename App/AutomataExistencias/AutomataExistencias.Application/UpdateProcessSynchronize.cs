using System;
using AutomataExistencias.Core.Extensions;
using NLog;

namespace AutomataExistencias.Application
{
    public class UpdateProcessSynchronize : IUpdateProcessSynchronize
    {
        private readonly Logger _logger;
        private readonly Domain.Cataprom.IUpdateProcessService _catapromUpdateProcessService;
        
        public UpdateProcessSynchronize(Domain.Cataprom.IUpdateProcessService catapromUpdateProcessService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _catapromUpdateProcessService = catapromUpdateProcessService;
        }
        
        public void UpdateProcess()
        {
            try
            {
                _catapromUpdateProcessService.Update();
            }
            catch (Exception ex)
            {
                _logger.Error($"Internal error when trying to update LastImportDate in sql {ex.ToJson()}");
            }
        }
    }
}