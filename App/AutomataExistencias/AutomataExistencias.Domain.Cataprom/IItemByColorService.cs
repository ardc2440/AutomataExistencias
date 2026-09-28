using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IItemByColorService
    {
        [System.Obsolete("Carga la tabla completa del destino en memoria. No usar en el flujo del servicio.")]
        IEnumerable<ItemByColor> Get();
        void AddOrUpdate(ItemByColor item);
        void Remove(ItemByColor item);
        void Remove(List<ItemByColor> items);
        void SaveChanges();
    }
}
