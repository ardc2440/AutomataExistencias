using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IMoneyService
    {
        [System.Obsolete("Carga la tabla completa del destino en memoria. No usar en el flujo del servicio.")]
        IEnumerable<Money> Get();
        void AddOrUpdate(Money item);
        void Remove(Money item);
        void Remove(List<Money> items);
        void SaveChanges();
    }
}