using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IItemByColorService
    {
        IEnumerable<ItemByColor> Get();
        void AddOrUpdate(ItemByColor item);
        void Remove(ItemByColor item);
        void Remove(List<ItemByColor> items);
        void SaveChanges();
    }
}
