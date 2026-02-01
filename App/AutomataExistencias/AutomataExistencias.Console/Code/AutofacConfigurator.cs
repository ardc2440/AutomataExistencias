using Autofac;
using Microsoft.Extensions.Configuration;
using Autofac.Configuration;
using AutomataExistencias.DataAccess.Core;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.DataAccess.Core.Contract;
using AutomataExistencias.Application;
using AutomataExistencias.Console.Jobs;
using AutomataExistencias.Console.Schedules;

namespace AutomataExistencias.Console.Code
{
    internal static class AutofacConfigurator
    {
        private static readonly object _syncRoot = new object();
        private static IContainer _container;

        public static IContainer GetContainer()
        {
            if (_container != null) return _container;
            lock (_syncRoot)
            {
                if (_container != null) return _container;
                var config = new ConfigurationBuilder();
                var module = new ConfigurationModule(config.Build());
                var builder = new ContainerBuilder();
                builder.RegisterType<ApplicationConfigurator>().As<IConfigurator>();

            /*Context*/
            builder.RegisterType<AldebaranBaseContext>().InstancePerDependency();
            // CatapromBaseContext is created per-destination inside CatapromDestinationRunner using connection strings
            // do not register CatapromBaseContext here to avoid resolving it without a destination connection.

            /*Environments*/
            builder.RegisterType<AldebaranApplicationEnvironment>().As<IAldebaranApplicationEnvironment>();
            builder.RegisterType<CatapromApplicationEnvironment>().As<ICatapromApplicationEnvironment>();

            /*UnitOfWork*/
            builder.RegisterType<UnitOfWorkAldebaran>().As<IUnitOfWorkAldebaran>().InstancePerDependency();
            // UnitOfWorkCataprom is constructed manually per-destination inside CatapromDestinationRunner
            // so do not register a global IUnitOfWorkCataprom implementation here.

            /*Cataprom*/
            builder.RegisterType<Domain.Cataprom.MoneyService>().As<Domain.Cataprom.IMoneyService>();
            builder.RegisterType<Domain.Cataprom.ItemService>().As<Domain.Cataprom.IItemService>();
            builder.RegisterType<Domain.Cataprom.ItemByColorService>().As<Domain.Cataprom.IItemByColorService>();
            builder.RegisterType<Domain.Cataprom.LineService>().As<Domain.Cataprom.ILineService>();
            builder.RegisterType<Domain.Cataprom.UnitMeasuredService>().As<Domain.Cataprom.IUnitMeasuredService>();
            builder.RegisterType<Domain.Cataprom.TransitOrderService>().As<Domain.Cataprom.ITransitOrderService>();
            builder.RegisterType<Domain.Cataprom.StockService>().As<Domain.Cataprom.IStockService>();
            builder.RegisterType<Domain.Cataprom.UpdateProcessService>().As<Domain.Cataprom.IUpdateProcessService>();
            builder.RegisterType<Domain.Cataprom.PackagingService>().As<Domain.Cataprom.IPackagingService>();

            /*Aldebaran*/
            builder.RegisterType<Domain.Aldebaran.ItemByColorService>().As<Domain.Aldebaran.IItemByColorService>();
            builder.RegisterType<Domain.Aldebaran.ItemService>().As<Domain.Aldebaran.IItemService>();
            builder.RegisterType<Domain.Aldebaran.LineService>().As<Domain.Aldebaran.ILineService>();
            builder.RegisterType<Domain.Aldebaran.StockService>().As<Domain.Aldebaran.IStockService>();
            builder.RegisterType<Domain.Aldebaran.MoneyService>().As<Domain.Aldebaran.IMoneyService>();
            builder.RegisterType<Domain.Aldebaran.TransitOrderService>().As<Domain.Aldebaran.ITransitOrderService>();
            builder.RegisterType<Domain.Aldebaran.UnitMeasuredService>().As<Domain.Aldebaran.IUnitMeasuredService>();
            builder.RegisterType<Domain.Aldebaran.PackagingService>().As<Domain.Aldebaran.IPackagingService>();
            builder.RegisterType<Domain.Aldebaran.CleanService>().As<Domain.Aldebaran.ICleanService>();
            builder.RegisterType<Domain.Aldebaran.InventoryAutomationConnectionService>().As<Domain.Aldebaran.IInventoryAutomationConnectionService>();
            builder.RegisterType<Domain.Aldebaran.ItemsMasterService>().As<Domain.Aldebaran.IItemsMasterService>();
            builder.RegisterType<Domain.Aldebaran.RecoveryDomainService>().As<Domain.Aldebaran.IRecoveryDomainService>();
            builder.RegisterType<Domain.Aldebaran.AutomataNotificationRecipientService>().As<Domain.Aldebaran.IAutomataNotificationRecipientService>();
            builder.RegisterType<Domain.Aldebaran.AutomataConnectivityPatternService>().As<Domain.Aldebaran.IAutomataConnectivityPatternService>();
            // AutomataConnectivityThresholdService removed - thresholds are handled by global configuration
            // Automata state (shared in-memory state)
            builder.RegisterType<Core.AutomataState>().As<Core.IAutomataState>().SingleInstance();
            
            /* Connectivity classifier */
            builder.RegisterType<ConnectivityErrorClassifier>().As<IConnectivityErrorClassifier>().SingleInstance();
            builder.RegisterType<StartupRecoveryChecker>().As<IStartupRecoveryChecker>();
            builder.RegisterType<RecoveryService>().As<IRecoveryService>();
            
            /* Notification service */
            builder.RegisterType<NotificationService>().As<INotificationService>().SingleInstance();


            builder.RegisterType<Domain.Aldebaran.Homologacion.ItemsHomologadosService>().As<Domain.Aldebaran.Homologacion.IItemsHomologadosService>();
            builder.RegisterType<Domain.Aldebaran.Homologacion.ItemReferencesHomologadosService>().As<Domain.Aldebaran.Homologacion.IItemReferencesHomologadosService>();
            builder.RegisterType<Domain.Aldebaran.Homologacion.CurrenciesHomologadosService>().As<Domain.Aldebaran.Homologacion.ICurrenciesHomologadosService>();
            builder.RegisterType<Domain.Aldebaran.Homologacion.MeasureUnitsHomologadosService>().As<Domain.Aldebaran.Homologacion.IMeasureUnitsHomologadosService>();
            builder.RegisterType<Domain.Aldebaran.Homologacion.PackagingHomologadosService>().As<Domain.Aldebaran.Homologacion.IPackagingHomologadosService>();

            /*Sync*/
            builder.RegisterType<ItemByColorSynchronize>().As<IItemByColorSynchronize>();
            builder.RegisterType<ItemSynchronize>().As<IItemSynchronize>();
            builder.RegisterType<LineSynchronize>().As<ILineSynchronize>();
            builder.RegisterType<MoneySynchronize>().As<IMoneySynchronize>();
            builder.RegisterType<PackagingSynchronize>().As<IPackagingSynchronize>();
            builder.RegisterType<StockSynchronize>().As<IStockSynchronize>();
            builder.RegisterType<CatapromDestinationRunner>().As<ICatapromDestinationRunner>();
            builder.RegisterType<UpdateProcessSynchronize>().As<IUpdateProcessSynchronize>();
            builder.RegisterType<CleanerProcess>().As<ICleanerProcess>();
            builder.RegisterType<TransitOrderSynchronize>().As<ITransitOrderSynchronize>();
            builder.RegisterType<UnitMeasuredSynchronize>().As<IUnitMeasuredSynchronize>();
            // Sync orchestrator service used by Recovery to trigger immediate processing
            builder.RegisterType<SyncOrchestratorService>().As<ISyncOrchestrator>().SingleInstance();

            /*Jobs*/
            builder.RegisterType<JobSchedulerFactory>().As<IJobSchedulerFactory>();
            builder.RegisterType<NonConnectivityErrorsJob>().AsSelf();
            builder.RegisterType<SyncJob>().AsSelf();

                builder.RegisterModule(module);
                _container = builder.Build();
                return _container;
            }
        }
   }
}
