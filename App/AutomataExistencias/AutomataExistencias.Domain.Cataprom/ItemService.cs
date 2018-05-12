using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.DataAccess.Cataprom;
using AutomataExistencias.DataAccess.Core.Contract;
using NLog;

namespace AutomataExistencias.Domain.Cataprom
{
    public class ItemService : IItemService
    {
        #region Properties
        private readonly Logger _logger;
        private readonly IUnitOfWorkCataprom _unitOfWork;

        #endregion

        public ItemService(IUnitOfWorkCataprom unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public IEnumerable<Item> Get()
        {
            return _unitOfWork.Repository<Item>().Get();
        }

        public void AddOrUpdate(Item item)
        {
            var entity = _unitOfWork.Repository<Item>().Find(item.Id);
            if (entity == null)
            {
                _unitOfWork.Repository<Item>().Add(item);
                return;
            }
            entity.LineId = item.LineId;
            entity.Reference = item.Reference;
            entity.Name = item.Name;
            entity.ProviderReference = item.ProviderReference;
            entity.ProviderItemName = item.ProviderItemName;
            entity.ItemType = item.ItemType;
            entity.FobCost = item.FobCost;
            entity.MoneyId = item.MoneyId;
            entity.PartType = item.PartType;
            entity.Determinant = item.Determinant;
            entity.Observations = item.Observations;
            entity.StockExt = item.StockExt;
            entity.CifCost = item.CifCost;
            entity.Volume = item.Volume;
            entity.Weight = item.Weight;
            entity.FobUnitId = item.FobUnitId;
            entity.CifUnitId = item.CifUnitId;
            entity.NationalProduct = item.NationalProduct;
            entity.Active = item.Active;
            entity.VisibleCatalog = item.VisibleCatalog;
            _unitOfWork.Repository<Item>().Update(entity);
        }

        public void Remove(Item item)
        {
            var entity = _unitOfWork.Repository<Item>().Find(item.Id);
            if (entity == null)
            {
                _logger.Warn($"Item with Id [{item.Id}] does not exists");
                return;
            }
            _unitOfWork.Repository<Item>().Remove(entity);
        }

        public void Remove(List<Item> items)
        {
            foreach (var entity in items)
                Remove(entity);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<Item>().SaveChanges();
        }
    }
}