using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class PackagingService : IPackagingService
    {
        #region Properties

        private readonly IUnitOfWorkAldebaran _unitOfWork;

        #endregion

        public PackagingService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Packaging> Get()
        {
            return _unitOfWork.Repository<Packaging>().Get();
        }

        public void Remove(Packaging item)
        {
            _unitOfWork.Repository<Packaging>().Remove(item);
        }

        public void Remove(IEnumerable<Packaging> items)
        {
            _unitOfWork.Repository<Packaging>().Remove(items);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<Packaging>().SaveChanges();
        }
    }
}