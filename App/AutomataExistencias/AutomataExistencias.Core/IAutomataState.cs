using System;

namespace AutomataExistencias.Core
{
    public interface IAutomataState
    {
        bool IsDestinationConnectivityDown { get; set; }
        DateTime? DestinationConnectivityDownSince { get; set; }

        bool IsOriginConnectivityDown { get; set; }
        DateTime? OriginConnectivityDownSince { get; set; }
        
        void IncrementConnectivityError(string entityName, int connectionId);
        int GetConnectivityErrorCount(string entityName);
        System.Collections.Generic.IEnumerable<int> GetConnectionsWithErrors();
        void ResetConnectivityErrorCounts();
    }
}