using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
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
    public class Service : IDisposable
    {
        private readonly IJobSchedulerFactory _jobScheduler;
        private System.Threading.Timer _memoryTimer;

        public Service(IConfigurator configurator, IJobSchedulerFactory jobScheduler)
        {
            Logger = LogManager.GetCurrentClassLogger();
            try
            {
                Logger.Info("Initializing Components");
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
                var container = AutofacConfigurator.GetContainer();

                // Hotfix_CaidaServicio: validaciones movidas desde Program.Main para que
                // "install/uninstall" no dependan de la base de datos y un fallo se reporte como fallo de arranque.
                using (var scope = container.BeginLifetimeScope())
                {
                    var inventoryConnectionService = scope.Resolve<Domain.Aldebaran.IInventoryAutomationConnectionService>();
                    var activeConnections = inventoryConnectionService.GetActive();
                    if (activeConnections == null || !activeConnections.Any())
                    {
                        Logger.Error("No active Inventory Automation connections found. Service will not start.");
                        return false;
                    }
                }

                // Chequeo de recovery al arranque: solo si Recovery.Enabled=true (mismo codigo que antes estaba en Program.Main).
                // Se resuelve desde el contenedor raiz porque TryRunRecoveryOnStartup lanza una tarea en segundo plano.
                bool recoveryEnabled;
                if (!bool.TryParse(ConfigurationManager.AppSettings["Recovery.Enabled"], out recoveryEnabled) || recoveryEnabled)
                {
                    var recoveryChecker = container.Resolve<IStartupRecoveryChecker>();
                    if (recoveryChecker.ShouldRunRecoveryOnStartup())
                    {
                        Logger.Warn("StartupRecoveryChecker determined recovery should run before starting the agent. Executing lightweight recovery/notification.");
                        var ok = recoveryChecker.TryRunRecoveryOnStartup();
                        if (!ok)
                        {
                            Logger.Error("Startup recovery could not complete. Agent will not start until recovery is addressed.");
                            return false;
                        }
                    }
                }
                else
                {
                    Logger.Warn("Recovery.Enabled=false: se omite StartupRecoveryChecker.");
                }

                Task.Factory.StartNew(() =>
                {
                    _jobScheduler.Schedule();
                }, TaskCreationOptions.LongRunning)
                    .ContinueWith(t =>
                    {
                        Logger.Error(t.Exception.ToJson());
                        hc.Stop();
                    }, TaskContinuationOptions.OnlyOnFaulted);

                // Hotfix_CaidaServicio: registrar memoria cada minuto para detectar tendencias.
                _memoryTimer = new System.Threading.Timer(_ => LogMemory(), null, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1));

                NotifyServiceEvent("Servicio iniciado",
                    "El servicio AutomataExistencias inicio." + Environment.NewLine +
                    "Si nadie lo inicio manualmente, Windows lo reinicio despues de una caida: revisar Log_Error del dia.",
                    false);

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
            try
            {
                Logger.Info("Stopping service...");
                _memoryTimer?.Dispose();
                _memoryTimer = null;
                _jobScheduler?.Shutdown();
                NotifyServiceEvent("Servicio detenido",
                    "El servicio AutomataExistencias se detuvo de forma controlada (manual o por el sistema).",
                    true);
                Logger.Info("Service stopped.");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error stopping service: {ex}");
            }
            finally
            {
                LogManager.Flush(TimeSpan.FromSeconds(5));
            }
        }

        private void LogMemory()
        {
            try
            {
                using (var process = Process.GetCurrentProcess())
                {
                    Logger.Info($"[Memory] WorkingSet={process.WorkingSet64 / 1048576} MB | PrivateBytes={process.PrivateMemorySize64 / 1048576} MB | GCHeap={GC.GetTotalMemory(false) / 1048576} MB | Is64BitProcess={Environment.Is64BitProcess}");
                }
            }
            catch
            {
                // el log de memoria nunca debe afectar el servicio
            }
        }

        private void NotifyServiceEvent(string subject, string body, bool waitForSend)
        {
            try
            {
                AutofacConfigurator.GetContainer().Resolve<INotificationService>().NotifyServiceEvent(subject, body, waitForSend);
            }
            catch (Exception ex)
            {
                Logger.Warn($"Could not send service event notification '{subject}': {ex.Message}");
            }
        }

        public void Dispose()
        {
            _memoryTimer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
