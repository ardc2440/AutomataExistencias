using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AutomataExistencias.Core
{
    public class AutomataState : IAutomataState
    {
        public bool IsDestinationConnectivityDown { get; set; }
        public DateTime? DestinationConnectivityDownSince { get; set; }

        public bool IsOriginConnectivityDown { get; set; }
        public DateTime? OriginConnectivityDownSince { get; set; }

        // Sliding window data structures
        private readonly ConcurrentQueue<(DateTime Timestamp, int ConnectionId, bool IsConnectivityError)> _attemptEvents = new ConcurrentQueue<(DateTime, int, bool)>();
        private readonly ConcurrentDictionary<int, int> _consecutiveFailures = new ConcurrentDictionary<int, int>();

        public void RecordAttempt(int connectionId, bool isConnectivityError)
        {
            var now = DateTime.UtcNow;
            _attemptEvents.Enqueue((now, connectionId, isConnectivityError));

            if (isConnectivityError)
            {
                _consecutiveFailures.AddOrUpdate(connectionId, 1, (k, v) => v + 1);
            }
            else
            {
                _consecutiveFailures.TryRemove(connectionId, out _);
            }
        }

        // Backwards-compatible wrapper
        public void IncrementConnectivityError(string entityName, int connectionId)
        {
            RecordAttempt(connectionId, true);
        }

        private void TrimOldEvents(int windowMinutes)
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-windowMinutes);
            while (_attemptEvents.TryPeek(out var ev) && ev.Timestamp < cutoff)
            {
                // Dequeue old event
                if (_attemptEvents.TryDequeue(out var old))
                {
                    // If it was a connectivity error, we cannot decrement consecutiveFailures reliably here
                    // because consecutiveFailures represents consecutive runs; we keep it as-is and let RecoveryJob clear when appropriate.
                }
            }
        }

        public int GetTotalAttempts(int windowMinutes)
        {
            TrimOldEvents(windowMinutes);
            return _attemptEvents.Count;
        }

        public int GetConnectivityErrorCount(int windowMinutes)
        {
            TrimOldEvents(windowMinutes);
            return _attemptEvents.Where(e => e.IsConnectivityError).Count();
        }

        public double GetConnectivityErrorPercentage(int windowMinutes)
        {
            TrimOldEvents(windowMinutes);
            var total = _attemptEvents.Count;
            if (total == 0) return 0;
            var errors = _attemptEvents.Where(e => e.IsConnectivityError).Count();
            return (errors * 100.0) / total;
        }

        public IEnumerable<int> GetConnectionsWithErrors()
        {
            return _consecutiveFailures.Keys;
        }

        public int GetConsecutiveFailures(int connectionId)
        {
            _consecutiveFailures.TryGetValue(connectionId, out var v);
            return v;
        }

        public void ClearErrorsForConnection(int connectionId)
        {
            _consecutiveFailures.TryRemove(connectionId, out _);
            // Also remove events for this connection from the queue (inefficient but acceptable for moderate throughput)
            var tmp = new Queue<(DateTime, int, bool)>();
            while (_attemptEvents.TryDequeue(out var ev))
            {
                if (ev.ConnectionId != connectionId)
                    tmp.Enqueue(ev);
            }
            foreach (var e in tmp)
                _attemptEvents.Enqueue(e);
        }

        public int GetTotalConnectivityErrorCount()
        {
            return _attemptEvents.Where(e => e.IsConnectivityError).Count();
        }

        public void ResetConnectivityErrorCounts()
        {
            while (_attemptEvents.TryDequeue(out var _)) { }
            _consecutiveFailures.Clear();
        }
    }
}