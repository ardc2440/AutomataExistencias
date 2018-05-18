using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.DataAccess.Core;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Test.Code
{
    public class AutofacConfigurator
    {
        protected IContainer Container;

        public AutofacConfigurator()
        {
            var builder = new ContainerBuilder();
            /*Context*/
            builder.RegisterType<AldebaranBaseContext>().InstancePerDependency();
            builder.RegisterType<CatapromBaseContext>().InstancePerDependency();

            /*Environments*/
            builder.RegisterType<AldebaranApplicationEnvironment>().As<IAldebaranApplicationEnvironment>();
            builder.RegisterType<CatapromApplicationEnvironment>().As<ICatapromApplicationEnvironment>();

            /*UnitOfWork*/
            builder.RegisterType<ApplicationConfigurator>().As<IConfigurator>();
            builder.RegisterType<UnitOfWorkAldebaran>().As<IUnitOfWorkAldebaran>();
            builder.RegisterType<UnitOfWorkCataprom>().As<IUnitOfWorkCataprom>();

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

            /*Sync*/
            builder.RegisterType<ItemByColorSynchronize>().As<IItemByColorSynchronize>();
            builder.RegisterType<ItemSynchronize>().As<IItemSynchronize>();
            builder.RegisterType<LineSynchronize>().As<ILineSynchronize>();
            builder.RegisterType<MoneySynchronize>().As<IMoneySynchronize>();
            builder.RegisterType<PackagingSynchronize>().As<IPackagingSynchronize>();
            builder.RegisterType<StockSynchronize>().As<IStockSynchronize>();
            builder.RegisterType<UpdateProcessSynchronize>().As<IUpdateProcessSynchronize>();
            builder.RegisterType<TransitOrderSynchronize>().As<ITransitOrderSynchronize>();
            builder.RegisterType<UnitMeasuredSynchronize>().As<IUnitMeasuredSynchronize>();

            Container = builder.Build();
        }
        public IContainer GetContainer()
        {
            return Container;
        }
    }
}
