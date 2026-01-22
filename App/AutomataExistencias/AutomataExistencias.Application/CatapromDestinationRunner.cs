using System;
using AutomataExistencias.DataAccess.Core;
using AutomataExistencias.DataAccess.Core.Contract;
using AutomataExistencias.Domain.Aldebaran;

namespace AutomataExistencias.Application
{
    public class CatapromDestinationRunner : ICatapromDestinationRunner
    {
        private readonly IInventoryAutomationConnectionService _inventoryConnectionService;

        public CatapromDestinationRunner(IInventoryAutomationConnectionService inventoryConnectionService)
        {
            _inventoryConnectionService = inventoryConnectionService;
        }

        public void RunForAllDestinations(Action<IUnitOfWorkCataprom> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var connections = _inventoryConnectionService.GetActive();
            foreach (var connection in connections)
            {
                var connectionString = InventoryAutomationConnectionStringBuilder.Build(connection);
                using (var context = new CatapromBaseContext(connectionString))
                {
                    var unitOfWork = new UnitOfWorkCataprom(context);
                    action(unitOfWork);
                }
            }
        }
    }
}
