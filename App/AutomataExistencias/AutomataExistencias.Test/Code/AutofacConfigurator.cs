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
            builder.RegisterType<Domain.Cataprom.StockItemService>().As<Domain.Cataprom.IStockItemService>();
            builder.RegisterType<Domain.Cataprom.MoneyService>().As<Domain.Cataprom.IMoneyService>();

            /*Aldebaran*/
            builder.RegisterType<Domain.Aldebaran.ItemByColorService>().As<Domain.Aldebaran.IItemByColorService>();
            builder.RegisterType<Domain.Aldebaran.ItemService>().As<Domain.Aldebaran.IItemService>();
            builder.RegisterType<Domain.Aldebaran.LineService>().As<Domain.Aldebaran.ILineService>();
            builder.RegisterType<Domain.Aldebaran.StockService>().As<Domain.Aldebaran.IStockService>();
            builder.RegisterType<Domain.Aldebaran.MoneyService>().As<Domain.Aldebaran.IMoneyService>();
            builder.RegisterType<Domain.Aldebaran.TransitOrderService>().As<Domain.Aldebaran.ITransitOrderService>();
            builder.RegisterType<Domain.Aldebaran.UnitMeasuredService>().As<Domain.Aldebaran.IUnitMeasuredService>();

            /*Sync*/
            builder.RegisterType<Synchronize>().As<ISynchronize>();

            Container = builder.Build();
        }
        public IContainer GetContainer()
        {
            return Container;
        }
    }
}
