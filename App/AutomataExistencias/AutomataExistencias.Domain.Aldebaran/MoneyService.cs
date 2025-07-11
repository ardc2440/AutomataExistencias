﻿using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class MoneyService : IMoneyService
    {
        #region Properties
        private readonly IUnitOfWorkAldebaran _unitOfWork;
        #endregion

        public MoneyService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Money> Get()
        {
            return _unitOfWork.Repository<Money>().Get();
        }

        public IEnumerable<Money> Get(int attempts)
        {
            return _unitOfWork.Repository<Money>().Get(w => w.Attempts < attempts);
        }

        public void Remove(Money item)
        {
            _unitOfWork.Repository<Money>().Remove(item);
        }

        public void Update(Money item)
        {
            _unitOfWork.Repository<Money>().Update(item);
        }

        public void Remove(IEnumerable<Money> items)
        {
            _unitOfWork.Repository<Money>().Remove(items);
        }

        public void SaveChanges()
        {
            _unitOfWork.Repository<Money>().SaveChanges();
        }
    }
}