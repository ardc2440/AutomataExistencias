using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class InventoryAutomationConnectionService : IInventoryAutomationConnectionService
    {
        private readonly IUnitOfWorkAldebaran _unitOfWork;

        public InventoryAutomationConnectionService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<InventoryAutomationConnection> GetActive()
        {
            return _unitOfWork.Repository<InventoryAutomationConnection>()
                .Get(c => c.Active)
                .ToList();
        }
    }
}
