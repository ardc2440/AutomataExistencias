using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

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

        public void Remove(IEnumerable<ItemByColor> entities)
        {
            _unitOfWork.Repository<ItemByColor>().Remove(entities);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<ItemByColor>().SaveChanges();
        }
    }
}