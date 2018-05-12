using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IMoneyService
    {
        IEnumerable<Money> Get();
        void Remove(Money item);
        void Remove(IEnumerable<Money> items);
        void SaveChanges();
    }
}