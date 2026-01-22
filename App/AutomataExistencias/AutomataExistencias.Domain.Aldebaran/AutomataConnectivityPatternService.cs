using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class AutomataConnectivityPatternService : IAutomataConnectivityPatternService
    {
        private readonly IUnitOfWorkAldebaran _unitOfWork;

        public AutomataConnectivityPatternService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<AutomataConnectivityErrorPattern> GetActivePatternsForTarget(char target)
        {
            return _unitOfWork.Repository<AutomataConnectivityErrorPattern>()
                .Get(w => w.IsActive && (w.Target == "B" || w.Target == target.ToString()));
        }
    }
}
