using System;
using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Application
{
    public interface INotificationService
    {
        void NotifyConnectivityDown(IEnumerable<InventoryAutomationConnection> connections, DateTime since);
        void NotifyConnectivityRecovered(int itemsRecovered, DateTime since, DateTime until);
    }
}
