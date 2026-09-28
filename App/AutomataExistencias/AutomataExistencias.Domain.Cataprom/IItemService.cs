using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IItemService
    {
        [System.Obsolete("Carga la tabla completa del destino en memoria. No usar en el flujo del servicio.")]
        IEnumerable<Item> Get();
        void AddOrUpdate(Item item);
        void Remove(Item item);
        void Remove(List<Item> items);
        void SaveChanges();
    }
}