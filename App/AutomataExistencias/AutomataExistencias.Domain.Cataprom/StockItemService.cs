using AutomataExistencias.DataAccess.Cataprom;
using AutomataExistencias.DataAccess.Core.Contract;
using System.Collections.Generic;
using System.Linq;
using NLog;
namespace AutomataExistencias.Domain.Cataprom
{
    public class StockItemService : IStockItemService
    {
        #region Properties
        private readonly Logger _logger;
        private readonly IUnitOfWorkCataprom _unitOfWork;

        #endregion

        public StockItemService(IUnitOfWorkCataprom unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void AddOrUpdate(StockItem item)
        {
            var entity = _unitOfWork.Repository<StockItem>().Get(w => w.ColorItemId == item.ColorItemId && w.StorageCellar == item.StorageCellar).FirstOrDefault();
            if (entity == null)
            {
                _unitOfWork.Repository<StockItem>().Add(item);
                return;
            }
            entity.Quantity = item.Quantity;
            _unitOfWork.Repository<StockItem>().Update(entity);
        }

        public IEnumerable<StockItem> Get()
        {
            return _unitOfWork.Repository<StockItem>().Get();
        }

        private void Remove(StockItem entity)
        {
            var stockItem = _unitOfWork.Repository<StockItem>().GetWithoutNoTracking(w => w.ColorItemId == entity.ColorItemId && w.StorageCellar == entity.StorageCellar).FirstOrDefault();
            if (stockItem == null)
            {
                _logger.Warn($"StockItem with ColorItemId [{entity.ColorItemId}] and StorageCellar [{entity.StorageCellar}] does not exists");
                return;
            }
            _unitOfWork.Repository<StockItem>().Remove(entity);
        }

        public void Remove(List<StockItem> items)
        {
            foreach (var item in items)
                Remove(item);
        }

        public void SaveChanges()
        {
            _unitOfWork.SaveChanges();
        }
    }
}
