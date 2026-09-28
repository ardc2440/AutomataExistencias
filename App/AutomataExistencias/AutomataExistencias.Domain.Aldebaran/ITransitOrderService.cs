using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface ITransitOrderService
    {
        [System.Obsolete("Carga la tabla completa en memoria. Usar Get(int attempts), que filtra en SQL.")]
        IEnumerable<TransitOrder> Get();
        IEnumerable<TransitOrder> Get(int attempts);
        void Remove(TransitOrder item);
        void Update(TransitOrder item);
        void Remove(IEnumerable<TransitOrder> items);
        void SaveChanges();
    }
}