using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Aldebaran.Homologacion;
using AutomataExistencias.DataAccess.Core.Contract;
using NLog;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class ItemsMasterService : IItemsMasterService
    {
        private readonly IUnitOfWorkAldebaran _unitOfWork;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
            // Normalize incoming id: treat it as ItemIdHomologado and map to Aldebaran ItemId when possible.
            var sourceItemId = itemId;
            try
            {
                var homolog = _unitOfWork.Repository<ItemHomologado>().GetByWhere(h => h.ItemIdHomologado == itemId);
                if (homolog != null)
                {
                    _logger.Debug($"ItemsMasterService: mapping homologated id {itemId} -> source ItemId {homolog.ItemId}");
                    sourceItemId = homolog.ItemId;
                }
                else
                {
                    _logger.Debug($"ItemsMasterService: no homologation found for id {itemId}; assuming source id");
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error($"ItemsMasterService: error looking up ItemHomologado for homologated id {itemId}: {ex}");
            }

            var entity = _unitOfWork.Repository<CatalogItem>().GetByWhere(w => w.ItemId == sourceItemId);

            if (entity == null)
            {
                _logger.Warn($"ItemsMasterService: could not find CatalogItem for source id {sourceItemId} (incoming {itemId})");
                return;
            }

            entity.IsCatalogVisible = visible;
            _unitOfWork.Repository<CatalogItem>().Update(entity);
            _unitOfWork.Repository<CatalogItem>().SaveChanges();
        }
    }
}
