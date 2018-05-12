using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.DataAccess.Cataprom;
using AutomataExistencias.DataAccess.Core.Contract;
using NLog;

namespace AutomataExistencias.Domain.Cataprom
{
    public class MoneyService : IMoneyService
    {
        #region Properties
        private readonly Logger _logger;
        private readonly IUnitOfWorkCataprom _unitOfWork;

        #endregion

        public MoneyService(IUnitOfWorkCataprom unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public IEnumerable<Money> Get()
        {
            return _unitOfWork.Repository<Money>().Get();
        }

        public void AddOrUpdate(Money item)
        {
            var entity = _unitOfWork.Repository<Money>().Find(item.Id);
            if (entity == null)
            {
                _unitOfWork.Repository<Money>().Add(item);
                return;
            }
            entity.Name = item.Name;
            entity.Active = item.Active;
            _unitOfWork.Repository<Money>().Update(entity);
        }

        public void Remove(Money item)
        {
            var entity = _unitOfWork.Repository<Money>().Find(item.Id);
            if (entity == null)
            {
                _logger.Warn($"Money with Id [{item.Id}] does not exists");
                return;
            }
            _unitOfWork.Repository<Money>().Remove(entity);
        }

        public void Remove(List<Money> items)
        {
            foreach (var entity in items)
                Remove(entity);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<Money>().SaveChanges();
        }
    }
}
