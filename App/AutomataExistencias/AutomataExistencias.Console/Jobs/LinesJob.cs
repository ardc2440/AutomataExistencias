using Autofac;
using AutomataExistencias.Application;
using AutomataExistencias.Console.Code;
using Quartz;

namespace AutomataExistencias.Console.Jobs
{
    [DisallowConcurrentExecution]
    public class LinesJob : IJob
    {
        private readonly ISynchronize _synchronize;
        public LinesJob()
        {
            var container = AutofacConfigurator.GetContainer();
            _synchronize = container.Resolve<ISynchronize>();
        }
        public void Execute(IJobExecutionContext context)
        {
            _synchronize.LinesSync();
        }
    }
}