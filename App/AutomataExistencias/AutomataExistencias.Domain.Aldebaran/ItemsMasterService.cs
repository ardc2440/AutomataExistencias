using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class ItemsMasterService : IItemsMasterService
    {
        private readonly IUnitOfWorkAldebaran _unitOfWork;

        public ItemsMasterService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CatalogItem GetByItemId(int itemId)
        {
            return _unitOfWork.Repository<CatalogItem>().GetByWhere(w => w.ItemId == itemId);
        }

        public void UpdateVisibility(int itemId, bool visible)
        {
            var entity = _unitOfWork.Repository<CatalogItem>().GetByWhere(w => w.ItemId == itemId);
            if (entity == null)
                return;
            entity.IsCatalogVisible = visible;
            _unitOfWork.Repository<CatalogItem>().Update(entity);
            _unitOfWork.Repository<CatalogItem>().SaveChanges();
        }
    }
}
