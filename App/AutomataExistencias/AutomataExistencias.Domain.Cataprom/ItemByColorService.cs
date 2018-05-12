using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.DataAccess.Cataprom;
using AutomataExistencias.DataAccess.Core.Contract;
using NLog;

namespace AutomataExistencias.Domain.Cataprom
{
    public class ItemByColorService : IItemByColorService
    {
        #region Properties
        private readonly Logger _logger;
        private readonly IUnitOfWorkCataprom _unitOfWork;

        #endregion

        public ItemByColorService(IUnitOfWorkCataprom unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = LogManager.GetCurrentClassLogger();
        }
        public IEnumerable<ItemByColor> Get()
        {
            return _unitOfWork.Repository<ItemByColor>().Get();
        }

        public void AddOrUpdate(ItemByColor item)
        {
            var entity = _unitOfWork.Repository<ItemByColor>().Get(w => w.Id == item.Id).FirstOrDefault();
            if (entity == null)
            {
                _unitOfWork.Repository<ItemByColor>().Add(item);
                return;
            }
            entity.ItemId = item.ItemId;
            entity.ItemByColorReference = item.ItemByColorReference;
            entity.ItemByColorInternalReference = item.ItemByColorInternalReference;
            entity.ColorName = item.ColorName;
            entity.ProviderNomItemByColor = item.ProviderNomItemByColor;
            entity.Observations = item.Observations;
            entity.Color = item.Color;
            entity.QuantityOrder = item.QuantityOrder;
            entity.Quantity = item.Quantity;
            entity.QuantityReserved = item.QuantityReserved;
            entity.QuantityOrderPan = item.QuantityOrderPan;
            entity.QuantityPan = item.QuantityPan;
            entity.QuantityReservedPan = item.QuantityReservedPan;
            entity.Active = item.Active;
            entity.SoldOut = item.SoldOut;
            entity.QuantityProcess = item.QuantityProcess;
            _unitOfWork.Repository<ItemByColor>().Update(entity);
        }

        public void Remove(ItemByColor item)
        {
            var entity = _unitOfWork.Repository<ItemByColor>().Find(item.Id);
            if (entity == null) return;
            _unitOfWork.Repository<ItemByColor>().Remove(entity);
        }

        public void Remove(List<ItemByColor> items)
        {
            foreach (var entity in items)
                Remove(entity);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<ItemByColor>().SaveChanges();
        }
    }
}
