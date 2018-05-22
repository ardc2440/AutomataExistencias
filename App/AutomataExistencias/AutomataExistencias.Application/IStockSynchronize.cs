using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Application
{
    public interface IStockSynchronize
    {
        void Sync(IEnumerable<Stock> data, int syncAttempts);
        void ReverseSync(IEnumerable<Stock> data, int syncAttempts);
    }
}