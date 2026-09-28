using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IUnitMeasuredService
    {
        [System.Obsolete("Carga la tabla completa del destino en memoria. No usar en el flujo del servicio.")]
        IEnumerable<UnitMeasured> Get();
        void AddOrUpdate(UnitMeasured item);
        void Remove(UnitMeasured item);
        void Remove(List<UnitMeasured> items);
        void SaveChanges();
    }
}
