using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IUnitMeasuredService
    {
        IEnumerable<UnitMeasured> Get();
        void Remove(IEnumerable<UnitMeasured> entities);
        void SaveChanges();
    }
}