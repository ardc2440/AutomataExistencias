using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface ITransitOrderService
    {
        IEnumerable<TransitOrder> Get();
        void AddOrUpdate(TransitOrder item);
        void Remove(TransitOrder item);
        void Remove(List<TransitOrder> items);
        void SaveChanges();
    }
}
