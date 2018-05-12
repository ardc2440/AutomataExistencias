using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class ItemService : IItemService
    {
        #region Properties

        private readonly IUnitOfWorkAldebaran _unitOfWork;

        #endregion

        public ItemService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Item> Get()
        {
            return _unitOfWork.Repository<Item>().Get();
        }

        public void Remove(Item item)
        {
            _unitOfWork.Repository<Item>().Remove(item);
        }

        public void Remove(IEnumerable<Item> items)
        {
            _unitOfWork.Repository<Item>().Remove(items);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<Item>().SaveChanges();
        }
    }
}