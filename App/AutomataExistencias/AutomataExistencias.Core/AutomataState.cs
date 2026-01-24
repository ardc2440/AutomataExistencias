using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AutomataExistencias.Core
{
    public class AutomataState : IAutomataState
    {
        public bool IsDestinationConnectivityDown { get; set; }
        public DateTime? DestinationConnectivityDownSince { get; set; }

        public bool IsOriginConnectivityDown { get; set; }
        public DateTime? OriginConnectivityDownSince { get; set; }

        private readonly ConcurrentDictionary<string, int> _connectivityErrorCounts = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<int, byte> _connectionsWithErrors = new ConcurrentDictionary<int, byte>();

        public void IncrementConnectivityError(string entityName, int connectionId)
        {
            _connectivityErrorCounts.AddOrUpdate(entityName ?? "Global", 1, (k, v) => v + 1);
            _connectionsWithErrors.TryAdd(connectionId, 1);
        }

        public int GetConnectivityErrorCount(string entityName)
        {
            _connectivityErrorCounts.TryGetValue(entityName ?? "Global", out var v);
            return v;
        }

        public IEnumerable<int> GetConnectionsWithErrors()
        {
            return _connectionsWithErrors.Keys;
        }

        public void ResetConnectivityErrorCounts()
        {
            _connectivityErrorCounts.Clear();
            _connectionsWithErrors.Clear();
        }
    }
}