using System;
using System.Configuration;
using Autofac;
using AutomataExistencias.Console.Code;
using AutomataExistencias.Console.Schedules;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using NLog;
using Topshelf;

namespace AutomataExistencias.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                logger.Info($"Trying to start Service: [{ConfigurationManager.AppSettings["Service.ServiceName"]}]");
                var container = AutofacConfigurator.GetContainer();
                HostFactory.Run(x =>
                {
                    x.Service<Service>(s =>
                    {
                        s.ConstructUsing(name => new Service(container.Resolve<IConfigurator>(), container.Resolve<IJobSchedulerFactory>()));
                        s.WhenStarted((tc, hc) => tc.Start(hc));
                        s.WhenStopped(tc => tc.Stop());
                    });
                    x.RunAsLocalSystem();
                    x.StartManually();
                    x.SetDescription(ConfigurationManager.AppSettings["Service.Description"]);
                    x.SetDisplayName(ConfigurationManager.AppSettings["Service.DisplayName"]);
                    x.SetServiceName(ConfigurationManager.AppSettings["Service.ServiceName"]);
                });
            }
            catch (Exception ex)
            {
                logger.Error($"Error on trying to start Service [{ConfigurationManager.AppSettings["Service.ServiceName"]}] Exception:{ex.ToJson()}");
            }
        }
    }
}
