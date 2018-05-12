using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IStockService
    {
        IEnumerable<Stock> Get();
        void Remove(IEnumerable<Stock> entities);
        void SaveChanges();
    }
}
