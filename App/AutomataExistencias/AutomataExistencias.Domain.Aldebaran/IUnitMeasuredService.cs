using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IUnitMeasuredService
    {
        [System.Obsolete("Carga la tabla completa en memoria. Usar Get(int attempts), que filtra en SQL.")]
        IEnumerable<UnitMeasured> Get();
        IEnumerable<UnitMeasured> Get(int attempts);
        void Remove(UnitMeasured item);
        void Update(UnitMeasured item);
        void Remove(IEnumerable<UnitMeasured> items);
        void SaveChanges();
    }
}