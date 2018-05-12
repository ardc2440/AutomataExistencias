using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IMoneyService
    {
        IEnumerable<Money> Get();
        void AddOrUpdate(Money item);
        void Remove(Money item);
        void Remove(List<Money> items);
        void SaveChanges();
    }
}