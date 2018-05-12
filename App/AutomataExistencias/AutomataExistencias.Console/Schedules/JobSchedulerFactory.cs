using AutomataExistencias.Console.Jobs;
using AutomataExistencias.Core.Extensions;
using NLog;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using Quartz.Impl.Matchers;

namespace AutomataExistencias.Console.Schedules
{
    public class JobSchedulerFactory : IJobSchedulerFactory
    {
        private readonly Logger _logger;
        private static volatile IScheduler _schedulerInstance;
        public JobSchedulerFactory()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }
        private static IScheduler FactoryInstance
        {
            get
            {
                if (_schedulerInstance != null) return _schedulerInstance;
                var schedFact = new StdSchedulerFactory();
                _schedulerInstance = schedFact.GetScheduler();
                _schedulerInstance.Start();
                return _schedulerInstance;
            }
        }
        public void Schedule(Dictionary<string, TimeSpan> scheduleConfig)
        {
            TimeSpan schedule;
            //if (scheduleConfig.TryGetValue("StockJob.Schedule", out schedule))
            //    SetSchedule<StockJob>((int)schedule.TotalSeconds);
            //if (scheduleConfig.TryGetValue("ItemsJob.Schedule", out schedule))
            //    SetSchedule<ItemsJob>((int)schedule.TotalSeconds);
            //if (scheduleConfig.TryGetValue("ItemsByColorJob.Schedule", out schedule))
            //    SetSchedule<ItemsByColorJob>((int)schedule.TotalSeconds);
            //if (scheduleConfig.TryGetValue("LinesJob.Schedule", out schedule))
            //    SetSchedule<LinesJob>((int)schedule.TotalSeconds);
            if (scheduleConfig.TryGetValue("MoneyJob.Schedule", out schedule))
                SetSchedule<MoneyJob>((int)schedule.TotalSeconds);
            //if (scheduleConfig.TryGetValue("TransitOrderJob.Schedule", out schedule))
            //    SetSchedule<TransitOrderJob>((int)schedule.TotalSeconds);
            //if (scheduleConfig.TryGetValue("UnitMeasuredJob.Schedule", out schedule))
            //    SetSchedule<UnitMeasuredJob>((int)schedule.TotalSeconds);
            var listener = new JobListener("AutomatasJobs");
            FactoryInstance.ListenerManager.AddJobListener(listener, GroupMatcher<JobKey>.AnyGroup());
        }
        private void SetSchedule<T>(int seconds) where T : IJob
        {
            try
            {
                _logger.Info("SetSchedule [{0}] at [{1} seconds]", typeof(T), seconds);
                var job = JobBuilder.Create<T>().Build();
                var trigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(seconds)
                        .RepeatForever())
                        .Build();
                FactoryInstance.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {
                _logger.Error("Error on Scheduling Job [{0}]: {1}", (typeof(T)), ex.ToJson());
            }
        }
    }
}
