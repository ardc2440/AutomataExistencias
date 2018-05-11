using Autofac;
using AutomataExistencias.Console.Code;
using AutomataExistencias.Console.Schedules;
using AutomataExistencias.Core.Configuration;
using Topshelf;

namespace AutomataExistencias.Console
{
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    public class Program
    {
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
        public static void Main(string[] args)
        {
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
                x.SetDescription("AutomataExistencias Service");
                x.SetDisplayName("AutomataExistencias Service");
                x.SetServiceName("AutomataExistencias");
            });
        }
    }
}
