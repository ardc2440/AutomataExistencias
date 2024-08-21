using AutomataExistencias.DataAccess.Aldebaran.Homologacion;
using AutomataExistencias.DataAccess.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataExistencias.Domain.Aldebaran.Homologacion
{
    public class MeasureUnitsHomologadosService: IMeasureUnitsHomologadosService
    {
        #region Properties
        private readonly IUnitOfWorkAldebaran _unitOfWork;
        #endregion

        public MeasureUnitsHomologadosService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public MeasureUnitHomologado GetById(short id) 
        {
            return _unitOfWork.Repository<MeasureUnitHomologado>().GetByWhere(w => w.MeasureUnitId == id);
        }
    }
}
