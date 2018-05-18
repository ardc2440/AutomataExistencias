using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IStockService
    {
        IEnumerable<Stock> Get();
        IEnumerable<Stock> Get(int attempts);
        void Remove(Stock item);
        void Update(Stock item);
        void Remove(IEnumerable<Stock> items);
        void SaveChanges();
    }
}
