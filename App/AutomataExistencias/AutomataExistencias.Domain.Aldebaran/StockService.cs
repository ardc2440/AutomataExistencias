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

        public IEnumerable<Stock> Get(int attempts)
        {
            return _unitOfWork.Repository<Stock>().Get(w => w.Attempts < attempts);
        }

        public void Remove(Stock item)
        {
            _unitOfWork.Repository<Stock>().Remove(item);
        }

        public void Update(Stock item)
        {
            _unitOfWork.Repository<Stock>().Update(item);
        }

        public void Remove(IEnumerable<Stock> items)
        {
            _unitOfWork.Repository<Stock>().Remove(items);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<Stock>().SaveChanges();
        }
    }
}