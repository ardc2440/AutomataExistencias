using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IItemByColorService
    {
        IEnumerable<ItemByColor> Get();
        IEnumerable<ItemByColor> Get(int attempts);
        void Remove(ItemByColor item);
        void Update(ItemByColor item);
        void Remove(IEnumerable<ItemByColor> items);
        void SaveChanges();
    }
}