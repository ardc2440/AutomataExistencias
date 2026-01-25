using System;
using Autofac;
using NLog;
using Quartz;

namespace AutomataExistencias.Console.Jobs
{
    [DisallowConcurrentExecution]
    public class RecoveryJob : IJob
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var container = AutomataExistencias.Console.Code.AutofacConfigurator.GetContainer();
                var recovery = container.Resolve<AutomataExistencias.Application.IRecoveryService>();
                var ok = recovery.TryRecoverOnce();
                if (ok)
                    _logger.Info("RecoveryJob: recovery completed successfully.");
                else
                    _logger.Warn("RecoveryJob: recovery did not complete.");
            }
            catch (Exception ex)
            {
                _logger.Error($"RecoveryJob error: {ex}");
            }
        }
    }
}
