using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface ILineService
    {
        [System.Obsolete("Carga la tabla completa del destino en memoria. No usar en el flujo del servicio.")]
        IEnumerable<Line> Get();
        void AddOrUpdate(Line item);
        void Remove(Line item);
        void Remove(List<Line> items);
        void SaveChanges();
    }
}
