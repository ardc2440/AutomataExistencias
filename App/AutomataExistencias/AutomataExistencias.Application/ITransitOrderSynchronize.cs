using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Application
{
    public interface ITransitOrderSynchronize
    {
        void Sync(IEnumerable<TransitOrder> data, int syncAttempts);
        void ReverseSync(IEnumerable<TransitOrder> data, int syncAttempts);
    }
}