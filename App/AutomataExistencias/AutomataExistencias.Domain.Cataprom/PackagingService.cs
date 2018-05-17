using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;
using AutomataExistencias.DataAccess.Core.Contract;
using NLog;

namespace AutomataExistencias.Domain.Cataprom
{
    public class PackagingService : IPackagingService
    {
        public PackagingService(IUnitOfWorkCataprom unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public IEnumerable<Packaging> Get()
        {
            return _unitOfWork.Repository<Packaging>().Get();
        }

        public void AddOrUpdate(Packaging item)
        {
            var entity = _unitOfWork.Repository<Packaging>().Find(item.Id);
            if (entity == null)
            {
                _unitOfWork.Repository<Packaging>().Add(item);
                return;
            }
            entity.ItemId = item.ItemId;
            entity.Weight = item.Weight;
            entity.Height = item.Height;
            entity.Width = item.Width;
            entity.Long = item.Long;
            entity.Quantity = item.Quantity;
            _unitOfWork.Repository<Packaging>().Update(entity);
        }

        public void Remove(Packaging item)
        {
            var entity = _unitOfWork.Repository<Packaging>().Find(item.Id);
            if (entity == null)
            {
                _logger.Warn($"Packaging with Id [{item.Id}] does not exists");
                return;
            }
            _unitOfWork.Repository<Packaging>().Remove(entity);
        }

        public void Remove(List<Packaging> items)
        {
            foreach (var entity in items)
                Remove(entity);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<Packaging>().SaveChanges();
        }

        #region Properties

        private readonly Logger _logger;
        private readonly IUnitOfWorkCataprom _unitOfWork;

        #endregion
    }
}