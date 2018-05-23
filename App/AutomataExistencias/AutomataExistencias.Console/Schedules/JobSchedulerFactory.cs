using System;
using System.Configuration;
using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Console.Code;
using AutomataExistencias.Console.Jobs;
using AutomataExistencias.Core.Configuration;
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
        private readonly IConfigurator _configurator;
        public JobSchedulerFactory()
        {
            var container = AutofacConfigurator.GetContainer();
            _configurator = container.Resolve<IConfigurator>();
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

        private static Tuple<IJobDetail, ITrigger> SetDailySchedule<T>(string intervalKey) where T : IJob
        {
            var sch = ConfigurationManager.AppSettings[intervalKey];
            var schedule = TimeSpan.Parse(sch);
            var jobBuilder = JobBuilder.Create<T>().Build();
            var trigger = TriggerBuilder.Create()
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(schedule.Hours, schedule.Minutes))
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
            if (bool.Parse(_configurator.GetKey("Schedule.Cleaner.Active")))
            {
                var cleanerJobTuple = SetDailySchedule<CleanerJob>("Schedule.Cleaner");
                factoryInstance.ScheduleJob(cleanerJobTuple.Item1, cleanerJobTuple.Item2);
            }
            var listener = new JobChainingJobListener("AutomatasJobs");
            factoryInstance.ListenerManager.AddJobListener(listener, GroupMatcher<JobKey>.AnyGroup());
        }
    }
}