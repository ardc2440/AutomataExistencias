using AutomataExistencias.Console.Schedules;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomataExistencias.Console.Code;
using Topshelf;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.Core.Configuration;
using Topshelf.Autofac;

namespace AutomataExistencias.Console
{
    public class Service : IDisposable
    {
        private readonly IJobSchedulerFactory _jobScheduler;
        private readonly Dictionary<string, TimeSpan> _jobSchedules = new Dictionary<string, TimeSpan>();
        public Service(IConfigurator configurator, IJobSchedulerFactory jobScheduler)
        {
            Logger = LogManager.GetCurrentClassLogger();
            try
            {
                Logger.Info("Initializing Components");
                _jobSchedules.Add("SyncJob", TimeSpan.Parse(configurator.GetKey("SyncJobSchedule")));
                _jobScheduler = jobScheduler;
            }
            catch (Exception ex)
            {
                Logger.Error("{0}|{1}", ex.Message, ex);
            }
        }

        protected Logger Logger { get; set; }

        public bool Start(HostControl hc)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    AutofacConfigurator.GetContainer();
                    _jobScheduler.Schedule(_jobSchedules);
                }, TaskCreationOptions.LongRunning)
                    .ContinueWith(t =>
                    {
                        Logger.Error(t.Exception.ToJson());
                        hc.Stop();
                    }, TaskContinuationOptions.OnlyOnFaulted);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToJson());
                throw;
            }
            return true;
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
