using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IUnitMeasuredService
    {
        IEnumerable<UnitMeasured> Get();
        IEnumerable<UnitMeasured> Get(int attempts);
        void Remove(UnitMeasured item);
        void Update(UnitMeasured item);
        void Remove(IEnumerable<UnitMeasured> items);
        void SaveChanges();
    }
}