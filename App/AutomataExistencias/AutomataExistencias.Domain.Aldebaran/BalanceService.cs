using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;
using System.Collections.Generic;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class BalanceService : IBalanceService
    {
        #region Properties

        private readonly IUnitOfWorkAldebaran _unitOfWork;

        #endregion

        public BalanceService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Balance> Get()
        {
            return _unitOfWork.Repository<Balance>().Get();
        }

        public void Remove(IEnumerable<Balance> entities)
        {
            _unitOfWork.Repository<Balance>().Remove(entities);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<Balance>().SaveChanges();
        }
    }
}
