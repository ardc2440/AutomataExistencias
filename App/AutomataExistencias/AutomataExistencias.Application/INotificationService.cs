using System;
using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Application
{
    public interface INotificationService
    {
        // Notify that connectivity to destinations is down. Optionally include the list of affected items
        // and the number of consecutive failed connectivity checks (for periodic alerts).
        void NotifyConnectivityDown(IEnumerable<InventoryAutomationConnection> connections, DateTime since, IEnumerable<AutomataExistencias.DataAccess.Aldebaran.Item> failedItems = null, int consecutiveFailures = 0);

        // Notify that connectivity has been recovered and how many items were processed
        void NotifyConnectivityRecovered(int itemsRecovered, DateTime since, DateTime until);

        // Informational notification about events in R* tables that reached attempts >= syncAttempts
        // but are NOT classified as connectivity errors. Items must include name and internal reference.
        void NotifyNonConnectivityErrors(IEnumerable<AutomataExistencias.DataAccess.Aldebaran.Item> items, DateTime since);
    }
}
