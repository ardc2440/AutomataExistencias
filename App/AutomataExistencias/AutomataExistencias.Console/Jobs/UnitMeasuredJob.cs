using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Console.Code;
using Quartz;

namespace AutomataExistencias.Console.Jobs
{
    [DisallowConcurrentExecution]
    public class UnitMeasuredJob : IJob
    {
        private readonly ISynchronize _synchronize;

        public UnitMeasuredJob()
        {
            var container = AutofacConfigurator.GetContainer();
            _synchronize = container.Resolve<ISynchronize>();
        }

        public void Execute(IJobExecutionContext context)
        {
            _synchronize.UnitMeasuredSync();
        }
    }
}