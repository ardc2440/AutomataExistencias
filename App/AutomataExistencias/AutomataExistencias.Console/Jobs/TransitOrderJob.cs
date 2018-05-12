using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Console.Code;
using Quartz;

namespace AutomataExistencias.Console.Jobs
{
    [DisallowConcurrentExecution]
    public class TransitOrderJob : IJob
    {
        private readonly ISynchronize _synchronize;

        public TransitOrderJob()
        {
            var container = AutofacConfigurator.GetContainer();
            _synchronize = container.Resolve<ISynchronize>();
        }

        public void Execute(IJobExecutionContext context)
        {
            System.Console.WriteLine("_synchronize.TransitOrderSync()");
            System.Threading.Thread.Sleep(10000);
        }
    }
}