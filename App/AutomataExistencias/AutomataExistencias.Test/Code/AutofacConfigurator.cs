using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.DataAccess.Core;
using AutomataExistencias.DataAccess.Core.Contract;
using AutomataExistencias.Domain.Aldebaran;
using AutomataExistencias.Domain.Cataprom;

namespace AutomataExistencias.Test.Code
{
    public class AutofacConfigurator
    {
        protected IContainer Container;

        public AutofacConfigurator()
        {
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
            Container = builder.Build();
        }
        public IContainer GetContainer()
        {
            return Container;
        }
    }
}
