using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IStockService
    {
        IEnumerable<Stock> Get();
        void AddOrUpdate(Stock item);
        void Remove(Stock item);
        void Remove(List<Stock> items);
        void SaveChanges();
    }
}
