using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IItemByColorService
    {
        IEnumerable<ItemByColor> Get();
        void Remove(IEnumerable<ItemByColor> entities);
        void SaveChanges();
    }
}