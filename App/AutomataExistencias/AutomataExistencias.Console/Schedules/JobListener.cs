using System;
using AutomataExistencias.Core.Extensions;
using NLog;
using Quartz;

namespace AutomataExistencias.Console.Schedules
{
    public class JobListener : IJobListener
    {
        private readonly Logger _logger;

        public JobListener(string name)
        {
            Name = name;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void JobToBeExecuted(IJobExecutionContext context)
        {
            _logger.Info("{0} Start at:> {1}", context.JobInstance, DateTime.Now);
        }

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            _logger.Info("{0} ExecutionVetoed at:> {1}", context.JobInstance, DateTime.Now);
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            _logger.Info("{0} Ends at:> {1} Elapsed TimeSpan [{2}]", context.JobInstance, DateTime.Now, context.JobRunTime);
            if (jobException == null) return;
            _logger.Error("JobExecutedException at [{0}] [{1}]", context.JobInstance, jobException.InnerException.ToJson());
        }

        public string Name { get; }
    }
}