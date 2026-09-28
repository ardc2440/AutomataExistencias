namespace AutomataExistencias.Console.Schedules
{
    public interface IJobSchedulerFactory
    {
        void Schedule();
        // Hotfix_CaidaServicio: detener Quartz de forma ordenada al parar el servicio.
        void Shutdown();
    }
}