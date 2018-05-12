using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class UnitMeasuredService : IUnitMeasuredService
    {
        #region Properties

        private readonly IUnitOfWorkAldebaran _unitOfWork;

        #endregion

        public UnitMeasuredService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<UnitMeasured> Get()
        {
            return _unitOfWork.Repository<UnitMeasured>().Get();
        }

        public void Remove(UnitMeasured item)
        {
            _unitOfWork.Repository<UnitMeasured>().Remove(item);
        }

        public void Remove(IEnumerable<UnitMeasured> items)
        {
            _unitOfWork.Repository<UnitMeasured>().Remove(items);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<UnitMeasured>().SaveChanges();
        }
    }
}