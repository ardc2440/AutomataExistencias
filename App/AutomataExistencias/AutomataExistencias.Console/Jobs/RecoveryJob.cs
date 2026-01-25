using System;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Console.Code;
using NLog;
using Quartz;

namespace AutomataExistencias.Console.Jobs
{
    [DisallowConcurrentExecution]
    public class RecoveryJob : IJob
    {
        private readonly IRecoveryService _recoveryService;
        private readonly Logger _logger;

        public RecoveryJob()
        {
            var container = AutofacConfigurator.GetContainer();
            _recoveryService = container.Resolve<IRecoveryService>();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Execute(IJobExecutionContext context)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Info("[RecoveryJob] has started");
            try
            {
                var ok = _recoveryService.TryRecoverOnce();
                _logger.Info($"[RecoveryJob] finished. Success={ok}");
            }
            catch (Exception ex)
            {
                _logger.Error($"An exception has occurred while execution of RecoveryJob | Exception: {ex}");
            }
            finally
            {
                watch.Stop();
                var elapsedMs = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
                _logger.Info($"[RecoveryJob] has finished in {elapsedMs}");
            }
        }
    }
}
