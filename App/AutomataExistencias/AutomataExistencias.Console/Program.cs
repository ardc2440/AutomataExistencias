using System;
using System.Configuration;
using Autofac;
using AutomataExistencias.Console.Code;
using AutomataExistencias.Console.Schedules;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Core.Extensions;
using NLog;
using Topshelf;
using System.Linq;

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

                // Validate that there is at least one active destination connection before starting the service
                var inventoryConnectionService = container.Resolve<Domain.Aldebaran.IInventoryAutomationConnectionService>();
                var activeConnections = inventoryConnectionService.GetActive();
                if (activeConnections == null || !activeConnections.Any())
                {
                    logger.Error("No active Inventory Automation connections found. Service will not start.");
                    return;
                }

                // Startup recovery check: if many pending connectivity errors exist, trigger recovery flow before starting agent
                var recoveryChecker = container.Resolve<AutomataExistencias.Application.IStartupRecoveryChecker>();
                if (recoveryChecker.ShouldRunRecoveryOnStartup())
                {
                    logger.Warn("StartupRecoveryChecker determined recovery should run before starting the agent. Executing lightweight recovery/notification.");
                    var ok = recoveryChecker.TryRunRecoveryOnStartup();
                    if (!ok)
                    {
                        logger.Error("Startup recovery could not complete. Agent will not start until recovery is addressed.");
                        return;
                    }
                }

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
