using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IItemService
    {
        IEnumerable<Item> Get();
        void Remove(IEnumerable<Item> entities);
        void SaveChanges();
    }
}