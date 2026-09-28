using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface IStockService
    {
        [System.Obsolete("Carga la tabla completa del destino en memoria. No usar en el flujo del servicio.")]
        IEnumerable<Stock> Get();
        void AddOrUpdate(Stock item);
        void Remove(Stock item);
        void Remove(List<Stock> items);
        void SaveChanges();
    }
}
