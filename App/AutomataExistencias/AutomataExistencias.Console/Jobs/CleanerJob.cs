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
        private readonly ICleanerProcess _cleanerProcess;
        private readonly Logger _logger;
        private readonly int _daysToKeep;
        public CleanerJob()
        {
            var container = AutofacConfigurator.GetContainer();
            _cleanerProcess = container.Resolve<ICleanerProcess>();
            var configurator = container.Resolve<IConfigurator>();
            _daysToKeep = configurator.GetKey("Cleaner.DaysToKeep").ToInt();
            _logger = LogManager.GetCurrentClassLogger();
        }
        public void Execute(IJobExecutionContext context)
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
