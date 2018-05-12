using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IItemService
    {
        IEnumerable<Item> Get();
        void AddOrUpdate(Item item);
        void Remove(Item item);
        void Remove(List<Item> items);
        void SaveChanges();
    }
}