using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IUnitMeasuredService
    {
        IEnumerable<UnitMeasured> Get();
        void AddOrUpdate(UnitMeasured item);
        void Remove(UnitMeasured item);
        void Remove(List<UnitMeasured> items);
        void SaveChanges();
    }
}
