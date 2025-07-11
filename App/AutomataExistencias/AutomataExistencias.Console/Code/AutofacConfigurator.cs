﻿using Autofac;
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
        public static IContainer GetContainer()
        {
            var config = new ConfigurationBuilder();
            var module = new ConfigurationModule(config.Build());
            var builder = new ContainerBuilder();
            builder.RegisterType<ApplicationConfigurator>().As<IConfigurator>();

            /*Context*/
            builder.RegisterType<AldebaranBaseContext>().InstancePerDependency();
            builder.RegisterType<CatapromBaseContext>().InstancePerDependency();

            /*Environments*/
            builder.RegisterType<AldebaranApplicationEnvironment>().As<IAldebaranApplicationEnvironment>();
            builder.RegisterType<CatapromApplicationEnvironment>().As<ICatapromApplicationEnvironment>();

            /*UnitOfWork*/
            builder.RegisterType<UnitOfWorkAldebaran>().As<IUnitOfWorkAldebaran>().InstancePerDependency();
            builder.RegisterType<UnitOfWorkCataprom>().As<IUnitOfWorkCataprom>().InstancePerDependency();

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
            builder.RegisterType<UpdateProcessSynchronize>().As<IUpdateProcessSynchronize>();
            builder.RegisterType<CleanerProcess>().As<ICleanerProcess>();
            builder.RegisterType<TransitOrderSynchronize>().As<ITransitOrderSynchronize>();
            builder.RegisterType<UnitMeasuredSynchronize>().As<IUnitMeasuredSynchronize>();

            /*Jobs*/
            builder.RegisterType<JobSchedulerFactory>().As<IJobSchedulerFactory>();
            builder.RegisterType<SyncJob>().AsSelf();

            builder.RegisterModule(module);
            return builder.Build();
        }
   }
}
