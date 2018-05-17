using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IPackagingService
    {
        IEnumerable<Packaging> Get();
        void AddOrUpdate(Packaging item);
        void Remove(Packaging item);
        void Remove(List<Packaging> items);
        void SaveChanges();
    }
}