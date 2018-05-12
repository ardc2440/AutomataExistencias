using AutomataExistencias.Console.Jobs;
using AutomataExistencias.Core.Extensions;
using NLog;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        public void Schedule()
        {
            var sch = System.Configuration.ConfigurationManager.AppSettings["Schedule.Interval"];
            var schedule = TimeSpan.Parse(sch);

            var syncJob = JobBuilder.Create<SyncJob>().Build();
            var trigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds((int)schedule.TotalSeconds)
                        .RepeatForever())
                        .Build();
            var schedFact = new StdSchedulerFactory();
            var factoryInstance = schedFact.GetScheduler();
            factoryInstance.Start();
            factoryInstance.ScheduleJob(syncJob, trigger);

            var listener = new JobChainingJobListener("AutomatasJobs");
            factoryInstance.ListenerManager.AddJobListener(listener, GroupMatcher<JobKey>.AnyGroup());
        }
    }
}
