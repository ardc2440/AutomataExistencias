using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.DataAccess.Cataprom;
using AutomataExistencias.DataAccess.Core.Contract;
using NLog;

namespace AutomataExistencias.Domain.Cataprom
{
    public class UnitMeasuredService : IUnitMeasuredService
    {
        #region Properties
        private readonly Logger _logger;
        private readonly IUnitOfWorkCataprom _unitOfWork;

        #endregion

        public UnitMeasuredService(IUnitOfWorkCataprom unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public IEnumerable<UnitMeasured> Get()
        {
            return _unitOfWork.Repository<UnitMeasured>().Get();
        }

        public void AddOrUpdate(UnitMeasured item)
        {
            var entity = _unitOfWork.Repository<UnitMeasured>().Find(item.Id);
            if (entity == null)
            {
                _unitOfWork.Repository<UnitMeasured>().Add(item);
                return;
            }
            entity.Name = item.Name;
            entity.Active = item.Active;
            _unitOfWork.Repository<UnitMeasured>().Update(entity);
        }

        public void Remove(UnitMeasured item)
        {
            var entity = _unitOfWork.Repository<UnitMeasured>().Find(item.Id);
            if (entity == null)
            {
                _logger.Warn($"UnitMeasured with Id [{item.Id}] does not exists");
                return;
            }
            _unitOfWork.Repository<UnitMeasured>().Remove(entity);
        }

        public void Remove(List<UnitMeasured> items)
        {
            foreach (var entity in items)
                Remove(entity);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<UnitMeasured>().SaveChanges();
        }
    }
}
