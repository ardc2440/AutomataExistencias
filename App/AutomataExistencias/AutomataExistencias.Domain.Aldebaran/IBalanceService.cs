using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IBalanceService
    {
        IEnumerable<Balance> Get();
        void Remove(IEnumerable<Balance> entities);
        void SaveChanges();
    }
}