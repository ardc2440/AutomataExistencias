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
            // EF cannot translate calls like target.ToString() inside expression trees.
            // Compute the target string outside the expression so it becomes a parameter.
            var targetStr = target.ToString();
            return _unitOfWork.Repository<AutomataConnectivityErrorPattern>()
                .Get(w => w.IsActive && (w.Target == "B" || w.Target == targetStr));
        }
    }
}
