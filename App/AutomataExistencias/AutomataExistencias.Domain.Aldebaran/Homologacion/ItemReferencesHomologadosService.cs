using AutomataExistencias.DataAccess.Aldebaran.Homologacion;
using AutomataExistencias.DataAccess.Core.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataExistencias.Domain.Aldebaran.Homologacion
{
    public class ItemReferencesHomologadosService: IItemReferencesHomologadosService
    {
        #region Properties
        private readonly IUnitOfWorkAldebaran _unitOfWork;
        #endregion

        public ItemReferencesHomologadosService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ItemReferenceHomologado GetById(int id) 
        {
            return _unitOfWork.Repository<ItemReferenceHomologado>().GetByWhere(w => w.ReferenceId == id);
        }
    }
}
