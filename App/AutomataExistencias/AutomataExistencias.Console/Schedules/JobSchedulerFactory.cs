using System;
using System.Configuration;
using AutomataExistencias.Console.Jobs;
using NLog;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Listener;

namespace AutomataExistencias.Console.Schedules
{
    public class JobSchedulerFactory : IJobSchedulerFactory
    {
        private readonly Logger _logger;

        public JobSchedulerFactory()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        private static Tuple<IJobDetail, ITrigger> SetSchedule<T>(string intervalKey) where T : IJob
        {
            var sch = ConfigurationManager.AppSettings[intervalKey];
            var schedule = TimeSpan.Parse(sch);
            var jobBuilder = JobBuilder.Create<T>().Build();
            var trigger = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds((int)schedule.TotalSeconds)
                    .RepeatForever())
                .Build();
            return new Tuple<IJobDetail, ITrigger>(jobBuilder, trigger);
        }
        public void Schedule()
        {
            var syncJobTuple = SetSchedule<SyncJob>("Schedule.Interval");
            var schedFact = new StdSchedulerFactory();
            var factoryInstance = schedFact.GetScheduler();
            factoryInstance.Start();
            factoryInstance.ScheduleJob(syncJobTuple.Item1, syncJobTuple.Item2);
            var listener = new JobChainingJobListener("AutomatasJobs");
            factoryInstance.ListenerManager.AddJobListener(listener, GroupMatcher<JobKey>.AnyGroup());
        }
    }
}