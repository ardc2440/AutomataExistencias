namespace AutomataExistencias.Application
{
    public interface IConnectivityErrorClassifier
    {
        bool IsDestinationConnectivityError(string exceptionText);
        bool IsOriginConnectivityError(string exceptionText);
        void Refresh();
    }
}
