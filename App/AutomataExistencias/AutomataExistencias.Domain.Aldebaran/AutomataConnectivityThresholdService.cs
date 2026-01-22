using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class AutomataConnectivityThresholdService : IAutomataConnectivityThresholdService
    {
        private readonly IUnitOfWorkAldebaran _unitOfWork;

        public AutomataConnectivityThresholdService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public AutomataConnectivityThreshold GetThresholdForEntity(string entityName)
        {
            var entity = entityName?.ToUpper();
            var threshold = _unitOfWork.Repository<AutomataConnectivityThreshold>()
                .Get(w => w.IsActive && w.EntityName.ToUpper() == entity)
                .FirstOrDefault();

            if (threshold != null) return threshold;

            return _unitOfWork.Repository<AutomataConnectivityThreshold>()
                .Get(w => w.IsActive && w.EntityName.ToUpper() == "GLOBAL").FirstOrDefault();
        }

        public IEnumerable<AutomataConnectivityThreshold> GetAllActive()
        {
            return _unitOfWork.Repository<AutomataConnectivityThreshold>().Get(w => w.IsActive);
        }
    }
}
