using System;
using System.Configuration;
using System.Threading.Tasks;
using Autofac;
using AutomataExistencias.Application;
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

            // Hotfix_CaidaServicio: capturar cualquier excepcion que tumbe el proceso, dejar rastro y avisar.
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => OnUnhandledException(logger, e);
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                try { logger.Error($"UnobservedTaskException: {e.Exception}"); } catch { }
                e.SetObserved();
            };

            try
            {
                logger.Info($"Trying to start Service: [{ConfigurationManager.AppSettings["Service.ServiceName"]}]");
                var container = AutofacConfigurator.GetContainer();

                // Las validaciones de conexiones activas y el chequeo de recovery se movieron a Service.Start.
                var exitCode = HostFactory.Run(x =>
                {
                    x.Service<Service>(s =>
                    {
                        s.ConstructUsing(name => new Service(container.Resolve<IConfigurator>(), container.Resolve<IJobSchedulerFactory>()));
                        s.WhenStarted((tc, hc) => tc.Start(hc));
                        s.WhenStopped(tc => tc.Stop());
                    });
                    x.RunAsLocalSystem();
                    // Hotfix_CaidaServicio: antes StartManually -> tras un reinicio del servidor el servicio no subia.
                    x.StartAutomatically();
                    // Hotfix_CaidaServicio: si el proceso muere, Windows lo reinicia (1 min) y resetea el contador cada dia.
                    x.EnableServiceRecovery(r =>
                    {
                        r.RestartService(1);
                        r.RestartService(1);
                        r.RestartService(5);
                        r.SetResetPeriod(1);
                    });
                    x.SetDescription(ConfigurationManager.AppSettings["Service.Description"]);
                    x.SetDisplayName(ConfigurationManager.AppSettings["Service.DisplayName"]);
                    x.SetServiceName(ConfigurationManager.AppSettings["Service.ServiceName"]);
                });

                Environment.ExitCode = (int)exitCode;
            }
            catch (Exception ex)
            {
                logger.Error($"Error on trying to start Service [{ConfigurationManager.AppSettings["Service.ServiceName"]}] Exception:{ex.ToJson()}");
                Environment.ExitCode = 1;
            }
            finally
            {
                LogManager.Flush(TimeSpan.FromSeconds(5));
            }
        }

        private static void OnUnhandledException(Logger logger, UnhandledExceptionEventArgs e)
        {
            try { logger.Fatal($"UnhandledException (IsTerminating={e.IsTerminating}): {e.ExceptionObject}"); } catch { }

            if (e.IsTerminating)
            {
                try
                {
                    AutofacConfigurator.GetContainer().Resolve<INotificationService>().NotifyServiceEvent(
                        "Servicio detenido por error",
                        "El servicio AutomataExistencias se detuvo por una excepcion no controlada:" + Environment.NewLine +
                        e.ExceptionObject + Environment.NewLine + Environment.NewLine +
                        "Windows intentara reiniciarlo automaticamente.",
                        true);
                }
                catch
                {
                    // best effort: si no hay memoria o SMTP, el correo "Servicio iniciado" del reinicio servira de aviso
                }
            }

            try { LogManager.Flush(TimeSpan.FromSeconds(5)); } catch { }
        }
    }
}
