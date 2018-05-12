using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.DataAccess.Cataprom;
using AutomataExistencias.DataAccess.Core.Contract;
using NLog;

namespace AutomataExistencias.Domain.Cataprom
{
    public class TransitOrderService : ITransitOrderService
    {
        #region Properties
        private readonly Logger _logger;
        private readonly IUnitOfWorkCataprom _unitOfWork;

        #endregion

        public TransitOrderService(IUnitOfWorkCataprom unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public IEnumerable<TransitOrder> Get()
        {
            return _unitOfWork.Repository<TransitOrder>().Get();
        }

        public void AddOrUpdate(TransitOrder item)
        {
            var entity = _unitOfWork.Repository<TransitOrder>().Get(w => w.Id == item.Id).FirstOrDefault();
            if (entity == null)
            {
                _unitOfWork.Repository<TransitOrder>().Add(item);
                return;
            }
            entity.DeliveredDate = item.DeliveredDate;
            entity.DeliveredQuantity = item.DeliveredQuantity;
            entity.Date = item.Date;
            entity.Activity = item.Activity;
            entity.ColorItemId = item.ColorItemId;
            _unitOfWork.Repository<TransitOrder>().Update(entity);
        }

        public void Remove(TransitOrder item)
        {
            var entity = _unitOfWork.Repository<TransitOrder>().Find(item.Id);
            if (entity == null) return;
            _unitOfWork.Repository<TransitOrder>().Remove(entity);
        }

        public void Remove(List<TransitOrder> items)
        {
            foreach (var entity in items)
                Remove(entity);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<TransitOrder>().SaveChanges();
        }
    }
}
