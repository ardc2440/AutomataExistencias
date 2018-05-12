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


        private Tuple<JobKey, IJobDetail> Build(string key, bool durable)
        {
            switch (key)
            {
                case "MoneyJob":
                    var moneyJobKey = JobKey.Create("MoneyJob", "Pipeline");
                    var moneyJob = JobBuilder.Create<MoneyJob>().WithIdentity(moneyJobKey).StoreDurably(durable).Build();
                    return new Tuple<JobKey, IJobDetail>(moneyJobKey, moneyJob);
                case "UnitMeasuredJob":
                    var unitMeasuredJobKey = JobKey.Create("UnitMeasuredJob", "Pipeline");
                    var unitMeasuredJob = JobBuilder.Create<UnitMeasuredJob>().WithIdentity(unitMeasuredJobKey).StoreDurably(durable).Build();
                    return new Tuple<JobKey, IJobDetail>(unitMeasuredJobKey, unitMeasuredJob);
                case "LinesJob":
                    var linesJobKey = JobKey.Create("LinesJob", "Pipeline");
                    var linesJob = JobBuilder.Create<LinesJob>().WithIdentity(linesJobKey).StoreDurably(durable).Build();
                    return new Tuple<JobKey, IJobDetail>(linesJobKey, linesJob);
                case "ItemsJob":
                    var itemsJobKey = JobKey.Create("ItemsJob", "Pipeline");
                    var itemsJob = JobBuilder.Create<ItemsJob>().WithIdentity(itemsJobKey).StoreDurably(durable).Build();
                    return new Tuple<JobKey, IJobDetail>(itemsJobKey, itemsJob);
                case "ItemsByColorJob":
                    var itemsByColorJobKey = JobKey.Create("ItemsByColorJob", "Pipeline");
                    var itemsByColorJob = JobBuilder.Create<ItemsByColorJob>().WithIdentity(itemsByColorJobKey).StoreDurably(durable).Build();
                    return new Tuple<JobKey, IJobDetail>(itemsByColorJobKey, itemsByColorJob);
                case "TransitOrderJob":
                    var transitOrderJobKey = JobKey.Create("TransitOrderJob", "Pipeline");
                    var transitOrderJob = JobBuilder.Create<TransitOrderJob>().WithIdentity(transitOrderJobKey).StoreDurably(durable).Build();
                    return new Tuple<JobKey, IJobDetail>(transitOrderJobKey, transitOrderJob);
                case "StockJob":
                    var stockJobKey = JobKey.Create("StockJob", "Pipeline");
                    var stockJob = JobBuilder.Create<StockJob>().WithIdentity(stockJobKey).StoreDurably(durable).Build();
                    return new Tuple<JobKey, IJobDetail>(stockJobKey, stockJob);
                default:
                    _logger.Warn($"Key [{key}] not identified");
                    return null;
            }
        }

        public void Schedule()
        {
            var sch = System.Configuration.ConfigurationManager.AppSettings["Schedule.Interval"];
            var schedule = TimeSpan.Parse(sch);

            var trigger = TriggerBuilder.Create()
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds((int)schedule.TotalSeconds)
                        .RepeatForever())
                        .Build();

            var schedFact = new StdSchedulerFactory();
            var factoryInstance = schedFact.GetScheduler();
            factoryInstance.Start();

            var scheduleSequence = System.Configuration.ConfigurationManager.AppSettings["Schedule.Sequence"].Split(';').ToList();
            var listener = new JobChainingJobListener("AutomatasJobs");

            for (var i = 0; i < scheduleSequence.Count; i++)
            {
                var current = Build(scheduleSequence[i], i != 0);
                if (current == null) continue;
                if (i == 0)
                    factoryInstance.ScheduleJob(current.Item2, trigger);
                else
                    factoryInstance.AddJob(current.Item2, false, true);

                if (i + 1 >= scheduleSequence.Count) continue;
                var next = Build(scheduleSequence[i + 1],true);
                if (next == null) continue;
                listener.AddJobChainLink(current.Item1, next.Item1);
            }
            factoryInstance.ListenerManager.AddJobListener(listener, GroupMatcher<JobKey>.AnyGroup());
        }
    }
}
