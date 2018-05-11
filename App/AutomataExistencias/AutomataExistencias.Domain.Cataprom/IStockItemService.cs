using AutomataExistencias.DataAccess.Cataprom;
using System.Collections.Generic;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IStockItemService
    {
        IEnumerable<StockItem> Get();
        void AddOrUpdate(StockItem item);
        void Remove(List<StockItem> items);
        void SaveChanges();
    }
}
