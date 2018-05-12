using AutomataExistencias.Console.Schedules;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using AutomataExistencias.Console.Code;
using Topshelf;
using AutomataExistencias.Core.Extensions;
using AutomataExistencias.Core.Configuration;

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
                foreach (var key in ConfigurationManager.AppSettings.AllKeys
                    .Where(key => key.Contains(".Schedule")))
                {
                    _jobSchedules.Add(key, TimeSpan.Parse(configurator.GetKey(key)));
                }
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
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToJson());
                return false;
            }
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
