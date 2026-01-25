namespace AutomataExistencias.Application
{
    public interface IRecoveryService
    {
        // Attempts a single recovery cycle. Returns true if recovery completed and agent can start.
        bool TryRecoverOnce();
    }
}
