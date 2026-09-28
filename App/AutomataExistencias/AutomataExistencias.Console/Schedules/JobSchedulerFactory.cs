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
        private IScheduler _scheduler;
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
        public void Shutdown()
        {
            if (_scheduler != null && !_scheduler.IsShutdown)
            {
                _logger.Info("Shutting down Quartz scheduler (waiting for running jobs)...");
                _scheduler.Shutdown(true);
            }
        }

        public void Schedule()
        {
            var syncJobTuple = SetSchedule<SyncJob>("Schedule.Interval");
            var schedFact = new StdSchedulerFactory();
            var factoryInstance = schedFact.GetScheduler();
            _scheduler = factoryInstance;
            factoryInstance.Start();
            factoryInstance.ScheduleJob(syncJobTuple.Item1, syncJobTuple.Item2);
            // Schedule RecoveryJob every configured interval
            // Hotfix_CaidaServicio: interruptor de emergencia Recovery.Enabled (default true).
            bool recoveryEnabled;
            if (!bool.TryParse(_configurator.GetKey("Recovery.Enabled"), out recoveryEnabled) || recoveryEnabled)
            {
                var recoveryTuple = SetSchedule<RecoveryJob>("Recovery.Interval");
                factoryInstance.ScheduleJob(recoveryTuple.Item1, recoveryTuple.Item2);
            }
            else
            {
                _logger.Warn("Recovery.Enabled=false: RecoveryJob no se agenda y SyncJob no marcara DOWN.");
            }
            // Schedule NonConnectivityErrorsJob every configured interval (default 30 minutes)
            var nonConnTuple = SetSchedule<NonConnectivityErrorsJob>("Notification.NonConnectivityInterval");
            factoryInstance.ScheduleJob(nonConnTuple.Item1, nonConnTuple.Item2);
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