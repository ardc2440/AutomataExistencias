namespace AutomataExistencias.Application
{
    public interface IStartupRecoveryChecker
    {
        // Returns true if startup is allowed (no recovery needed)
        bool ShouldRunRecoveryOnStartup();

        // Attempts a lightweight recovery or notification. Returns true if after this it's OK to start.
        bool TryRunRecoveryOnStartup();
    }
}
