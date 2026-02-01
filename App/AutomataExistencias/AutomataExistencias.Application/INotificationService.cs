using System;
using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Application
{
    public interface INotificationService
    {
        // Notify that connectivity to destinations is down. Optionally include the list of affected items
        // and the number of consecutive failed connectivity checks (for periodic alerts).
        void NotifyConnectivityDown(IEnumerable<InventoryAutomationConnection> connections, DateTime since, IEnumerable<Item> failedItems = null, int consecutiveFailures = 0);

        // Notify that connectivity has been recovered and how many items were processed
        void NotifyConnectivityRecovered(int itemsRecovered, DateTime since, DateTime until);
        
                
        // Notify both non-connectivity (business) errors and pending connectivity errors.
        // nonConnectivityDescriptions: formatted descriptions for business errors.
        // pendingConnectivityDescriptions: formatted descriptions for connectivity-pending items.
        void NotifyPendingAndNonConnectivityErrors(IEnumerable<string> nonConnectivityDescriptions, IEnumerable<string> pendingConnectivityDescriptions, DateTime since);
    }
}
