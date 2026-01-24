using System;

namespace AutomataExistencias.Core
{
    public class AutomataState : IAutomataState
    {
        public bool IsDestinationConnectivityDown { get; set; }
        public DateTime? DestinationConnectivityDownSince { get; set; }

        public bool IsOriginConnectivityDown { get; set; }
        public DateTime? OriginConnectivityDownSince { get; set; }
    }
}