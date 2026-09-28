using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IItemService
    {
        [System.Obsolete("Carga la tabla completa en memoria. Usar Get(int attempts), que filtra en SQL.")]
        IEnumerable<Item> Get();
        IEnumerable<Item> Get(int attempts);
        /// <summary>Registros con Attempts &gt; 0. El filtro se ejecuta en SQL (no carga la tabla completa).</summary>
        IEnumerable<Item> GetWithAttempts();
        void Remove(Item item);
        void Update(Item item);
        void Remove(IEnumerable<Item> items);
        void SaveChanges();
    }
}