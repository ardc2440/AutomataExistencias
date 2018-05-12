using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class TransitOrderService : ITransitOrderService
    {
        #region Properties

        private readonly IUnitOfWorkAldebaran _unitOfWork;

        #endregion

        public TransitOrderService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<TransitOrder> Get()
        {
            return _unitOfWork.Repository<TransitOrder>().Get();
        }

        public void Remove(IEnumerable<TransitOrder> entities)
        {
            _unitOfWork.Repository<TransitOrder>().Remove(entities);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<TransitOrder>().SaveChanges();
        }
    }
}