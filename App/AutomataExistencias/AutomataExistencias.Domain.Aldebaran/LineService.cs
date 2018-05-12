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

        public void Remove(Line item)
        {
            _unitOfWork.Repository<Line>().Remove(item);
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