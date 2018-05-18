using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface ITransitOrderService
    {
        IEnumerable<TransitOrder> Get();
        IEnumerable<TransitOrder> Get(int attempts);
        void Remove(TransitOrder item);
        void Update(TransitOrder item);
        void Remove(IEnumerable<TransitOrder> items);
        void SaveChanges();
    }
}