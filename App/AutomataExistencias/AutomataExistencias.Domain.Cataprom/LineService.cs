using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.DataAccess.Cataprom;
using AutomataExistencias.DataAccess.Core.Contract;
using NLog;

namespace AutomataExistencias.Domain.Cataprom
{
    public class LineService: ILineService
    {
        #region Properties
        private readonly Logger _logger;
        private readonly IUnitOfWorkCataprom _unitOfWork;

        #endregion

        public LineService(IUnitOfWorkCataprom unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = LogManager.GetCurrentClassLogger();
        }
        public IEnumerable<Line> Get()
        {
            return _unitOfWork.Repository<Line>().Get();
        }

        public void AddOrUpdate(Line item)
        {
            var entity = _unitOfWork.Repository<Line>().Find(item.Id);
            if (entity == null)
            {
                _unitOfWork.Repository<Line>().Add(item);
                return;
            }
            entity.Code = item.Code;
            entity.Name = item.Name;
            entity.Daemon = item.Daemon;
            entity.Active = item.Active;
            _unitOfWork.Repository<Line>().Update(entity);
        }

        public void Remove(Line item)
        {
            var entity = _unitOfWork.Repository<Line>().Find(item.Id);
            if (entity == null)
            {
                _logger.Warn($"Line with Id [{item.Id}] does not exists");
                return;
            }
            _unitOfWork.Repository<Line>().Remove(entity);
        }

        public void Remove(List<Line> items)
        {
            foreach (var entity in items)
                Remove(entity);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<Line>().SaveChanges();
        }
    }
}
