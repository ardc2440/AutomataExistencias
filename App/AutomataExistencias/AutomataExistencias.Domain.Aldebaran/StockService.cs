using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class StockService : IStockService
    {
        #region Properties

        private readonly IUnitOfWorkAldebaran _unitOfWork;

        #endregion

        public StockService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Stock> Get()
        {
            return _unitOfWork.Repository<Stock>().Get();
        }

        public void Remove(IEnumerable<Stock> entities)
        {
            _unitOfWork.Repository<Stock>().Remove(entities);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<Stock>().SaveChanges();
        }
    }
}