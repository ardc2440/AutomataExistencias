using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface ITransitOrderService
    {
        IEnumerable<TransitOrder> Get();
        void Remove(IEnumerable<TransitOrder> entities);
        void SaveChanges();
    }
}