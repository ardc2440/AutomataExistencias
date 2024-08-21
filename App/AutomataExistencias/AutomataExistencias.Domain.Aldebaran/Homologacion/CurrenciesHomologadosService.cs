using AutomataExistencias.DataAccess.Aldebaran.Homologacion;
using AutomataExistencias.DataAccess.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataExistencias.Domain.Aldebaran.Homologacion
{
    public class CurrenciesHomologadosService: ICurrenciesHomologadosService
    {
        #region Properties
        private readonly IUnitOfWorkAldebaran _unitOfWork;
        #endregion

        public CurrenciesHomologadosService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CurrencyHomologado GetById(short id) 
        {
            return _unitOfWork.Repository<CurrencyHomologado>().GetByWhere(w => w.CurrencyId == id);
        }
    }
}
