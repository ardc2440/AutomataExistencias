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

        public void Remove(TransitOrder item)
        {
            _unitOfWork.Repository<TransitOrder>().Remove(item);
        }

        public void Remove(IEnumerable<TransitOrder> items)
        {
            _unitOfWork.Repository<TransitOrder>().Remove(items);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<TransitOrder>().SaveChanges();
        }
    }
}