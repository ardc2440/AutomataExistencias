using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface ITransitOrderService
    {
        [System.Obsolete("Carga la tabla completa del destino en memoria. No usar en el flujo del servicio.")]
        IEnumerable<TransitOrder> Get();
        void AddOrUpdate(TransitOrder item);
        void Remove(TransitOrder item);
        void Remove(List<TransitOrder> items);
        void SaveChanges();
    }
}
