namespace AutomataExistencias.Application
{
    public interface ISyncOrchestrator
    {
        void RunOnce(bool ignoreAutomataState = false);
    }
}
