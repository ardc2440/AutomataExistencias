using AutomataExistencias.DataAccess.Aldebaran.Homologacion;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran.Homologacion
{
    public class ItemsHomologadosService: IItemsHomologadosService
    {
        #region Properties
        private readonly IUnitOfWorkAldebaran _unitOfWork;
        #endregion

        public ItemsHomologadosService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ItemHomologado GetById(int id) 
        {
            return _unitOfWork.Repository<ItemHomologado>().GetByWhere(w => w.ItemId == id);
        }
    }
}
