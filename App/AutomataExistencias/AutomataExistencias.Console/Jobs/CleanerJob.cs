using System;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Console.Code;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using NLog;
using Quartz;

namespace AutomataExistencias.Console.Jobs
{
    [DisallowConcurrentExecution]
    public class CleanerJob : IJob
    {
        private ICleanerProcess _cleanerProcess;
        private Logger _logger;
        private int _daysToKeep;
        private void ResolveDependencies(ILifetimeScope container)
        {
            _cleanerProcess = container.Resolve<ICleanerProcess>();
            var configurator = container.Resolve<IConfigurator>();
            _daysToKeep = configurator.GetKey("Cleaner.DaysToKeep").ToInt();
            _logger = LogManager.GetCurrentClassLogger();
        }
        public void Execute(IJobExecutionContext context)
        {
            // Hotfix_CaidaServicio: cada ejecucion usa su propio lifetime scope de Autofac.
            // Al salir del using se liberan (Dispose) los DbContext creados en la ejecucion;
            // antes se resolvian desde el contenedor raiz y quedaban retenidos (fuga de memoria).
            try
            {
                using (var scope = AutofacConfigurator.GetContainer().BeginLifetimeScope())
                {
                    ResolveDependencies(scope);
                    ExecuteInternal();
                }
            }
            catch (Exception ex)
            {
                // No propagar a Quartz: la siguiente ejecucion debe dispararse normalmente.
                LogManager.GetCurrentClassLogger().Error($"[CleanerJob] unhandled error: {ex}");
            }
        }

        private void ExecuteInternal()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Info("[CleanJob] has started");
            try
            {
                _cleanerProcess.Clean(_daysToKeep);
            }
            catch (Exception ex)
            {
                _logger.Error($"An exception has occurred while execution of CleanJob | Exception: {ex.ToJson()}");
            }
            finally
            {
                watch.Stop();
                var elapsedMs = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds);
                _logger.Info($"[CleanJob] has finished in {elapsedMs.ToReadableString()}");
            }
        }
    }
}
