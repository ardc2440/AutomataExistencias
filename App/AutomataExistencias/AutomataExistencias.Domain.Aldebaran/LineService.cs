using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class LineService : ILineService
    {
        #region Properties
        private readonly IUnitOfWorkAldebaran _unitOfWork;
        #endregion

        public LineService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Line> Get()
        {
            return _unitOfWork.Repository<Line>().Get();
        }

        public IEnumerable<Line> Get(int attempts)
        {
            return _unitOfWork.Repository<Line>().Get(w => w.Attempts < attempts);
        }

        public void Remove(Line item)
        {
            _unitOfWork.Repository<Line>().Remove(item);
        }

        public void Update(Line item)
        {
            _unitOfWork.Repository<Line>().Update(item);
        }

        public void Remove(IEnumerable<Line> items)
        {
            _unitOfWork.Repository<Line>().Remove(items);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<Line>().SaveChanges();
        }
    }
}