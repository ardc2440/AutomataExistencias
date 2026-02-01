using System;

namespace AutomataExistencias.Core
{
    public interface IAutomataState
    {
        bool IsDestinationConnectivityDown { get; set; }
        DateTime? DestinationConnectivityDownSince { get; set; }

        
        void IncrementConnectivityError(string entityName, int connectionId);
        // Record an attempt for a connection. isConnectivityError indicates whether it was a connectivity error.
        void RecordAttempt(int connectionId, bool isConnectivityError);

        // Counts and metrics within a sliding window (minutes)
        int GetTotalAttempts(int windowMinutes);
        int GetConnectivityErrorCount(int windowMinutes);
        double GetConnectivityErrorPercentage(int windowMinutes);

        // Per-connection metrics (connectionId can be -1 for origin)
        int GetTotalAttemptsForConnection(int connectionId, int windowMinutes);
        int GetConnectivityErrorCountForConnection(int connectionId, int windowMinutes);
        double GetConnectivityErrorPercentageForConnection(int connectionId, int windowMinutes);

        System.Collections.Generic.IEnumerable<int> GetConnectionsWithErrors();
        int GetConsecutiveFailures(int connectionId);
        void ClearErrorsForConnection(int connectionId);
        void ResetConnectivityErrorCounts();
        int GetTotalConnectivityErrorCount();
        // Return human-readable debug info about current sliding-window events and per-connection counts
        string GetConnectivityDebugInfo(int windowMinutes);
    }
}