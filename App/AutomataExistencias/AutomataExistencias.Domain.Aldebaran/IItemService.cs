using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IItemService
    {
        IEnumerable<Item> Get();
        IEnumerable<Item> Get(int attempts);
        void Remove(Item item);
        void Update(Item item);
        void Remove(IEnumerable<Item> items);
        void SaveChanges();
    }
}