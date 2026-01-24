using System;

namespace AutomataExistencias.Core
{
    public interface IAutomataState
    {
        bool IsDestinationConnectivityDown { get; set; }
        DateTime? DestinationConnectivityDownSince { get; set; }

        bool IsOriginConnectivityDown { get; set; }
        DateTime? OriginConnectivityDownSince { get; set; }
    }
}