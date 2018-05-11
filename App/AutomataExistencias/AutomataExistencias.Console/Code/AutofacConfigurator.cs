using Autofac;
using Microsoft.Extensions.Configuration;
using Autofac.Configuration;
using AutomataExistencias.DataAccess.Core;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.Domain.Aldebaran;
using AutomataExistencias.Domain.Cataprom;
using AutomataExistencias.DataAccess.Core.Contract;
using AutomataExistencias.Application;
using AutomataExistencias.Console.Jobs;
using AutomataExistencias.Console.Schedules;

namespace AutomataExistencias.Console.Code
{
    internal static class AutofacConfigurator
    {
        public static IContainer GetContainer()
        {
            var config = new ConfigurationBuilder();
            var module = new ConfigurationModule(config.Build());
            var builder = new ContainerBuilder();
            builder.RegisterType<AldebaranBaseContext>().InstancePerDependency();
            builder.RegisterType<CatapromBaseContext>().InstancePerDependency();

            builder.RegisterType<ApplicationConfigurator>().As<IConfigurator>();
            builder.RegisterType<UnitOfWorkAldebaran>().As<IUnitOfWorkAldebaran>();
            builder.RegisterType<UnitOfWorkCataprom>().As<IUnitOfWorkCataprom>();

            builder.RegisterType<BalanceService>().As<IBalanceService>();
            builder.RegisterType<StockItemService>().As<IStockItemService>();

            builder.RegisterType<AldebaranApplicationEnvironment>().As<IAldebaranApplicationEnvironment>();
            builder.RegisterType<CatapromApplicationEnvironment>().As<ICatapromApplicationEnvironment>();

            builder.RegisterType<Synchronize>().As<ISynchronize>();

            builder.RegisterType<JobSchedulerFactory>().As<IJobSchedulerFactory>();

            builder.RegisterType<SyncJob>().AsSelf();

            builder.RegisterModule(module);
            return builder.Build();
        }
   }
}
