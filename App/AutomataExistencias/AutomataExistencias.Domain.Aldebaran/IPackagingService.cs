using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IPackagingService
    {
        IEnumerable<Packaging> Get();
        void Remove(Packaging item);
        void Remove(IEnumerable<Packaging> items);
        void SaveChanges();
    }
}