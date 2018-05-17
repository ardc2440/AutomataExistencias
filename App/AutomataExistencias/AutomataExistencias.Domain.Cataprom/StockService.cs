using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutomataExistencias.DataAccess.Cataprom;
using AutomataExistencias.DataAccess.Core.Contract;
using NLog;

namespace AutomataExistencias.Domain.Cataprom
{
    public class StockService : IStockService
    {
        #region Properties
        private readonly Logger _logger;
        private readonly IUnitOfWorkCataprom _unitOfWork;

        #endregion

        public StockService(IUnitOfWorkCataprom unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public IEnumerable<Stock> Get()
        {
            return _unitOfWork.Repository<Stock>().Get();
        }

        public void AddOrUpdate(Stock item)
        {
            var entity = _unitOfWork.Repository<Stock>().GetWithoutNoTracking(w => w.ColorItemId == item.ColorItemId && w.StorageCellar == item.StorageCellar).FirstOrDefault();
            if (entity == null)
            {
                _unitOfWork.Repository<Stock>().Add(item);
                return;
            }
            entity.ItemId = item.ItemId;
            entity.Color = item.Color;
            entity.Quantity = item.Quantity;
            _unitOfWork.Repository<Stock>().Update(entity);
        }

        public void Remove(Stock item)
        {
            var entity = _unitOfWork.Repository<Stock>().GetWithoutNoTracking(w => w.ColorItemId == item.ColorItemId && w.StorageCellar == item.StorageCellar).FirstOrDefault();
            if (entity == null)
            {
                _logger.Warn($"StockItem with ColorItemId [{item.ColorItemId}] and StorageCellar [{item.StorageCellar}] does not exists");
                return;
            }
            _unitOfWork.Repository<Stock>().Remove(entity);
        }

        public void Remove(List<Stock> items)
        {
            foreach (var entity in items)
                Remove(entity);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<Stock>().SaveChanges();
        }
    }
}
