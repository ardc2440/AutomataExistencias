using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;
using NLog;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class ItemByColorService : IItemByColorService
    {
        #region Properties
        private readonly IUnitOfWorkAldebaran _unitOfWork;
        #endregion

        public ItemByColorService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ItemByColor> Get()
        {
            return _unitOfWork.Repository<ItemByColor>().Get();
        }

        public IEnumerable<ItemByColor> Get(int attempts)
        {
            return _unitOfWork.Repository<ItemByColor>().Get(w => w.Attempts < attempts);
        }

        public void Remove(ItemByColor item)
        {
            _unitOfWork.Repository<ItemByColor>().Remove(item);
        }

        public void Update(ItemByColor item)
        {
            _unitOfWork.Repository<ItemByColor>().Update(item);
        }

        public void Remove(IEnumerable<ItemByColor> items)
        {
            _unitOfWork.Repository<ItemByColor>().Remove(items);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<ItemByColor>().SaveChanges();
        }
    }
}