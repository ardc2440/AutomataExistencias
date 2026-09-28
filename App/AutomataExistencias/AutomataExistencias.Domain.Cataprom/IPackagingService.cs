using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IPackagingService
    {
        [System.Obsolete("Carga la tabla completa del destino en memoria. No usar en el flujo del servicio.")]
        IEnumerable<Packaging> Get();
        void AddOrUpdate(Packaging item);
        void Remove(Packaging item);
        void Remove(List<Packaging> items);
        void SaveChanges();
    }
}