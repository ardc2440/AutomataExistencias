using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IMoneyService
    {
        [System.Obsolete("Carga la tabla completa en memoria. Usar Get(int attempts), que filtra en SQL.")]
        IEnumerable<Money> Get();
        IEnumerable<Money> Get(int attempts);
        void Remove(Money item);
        void Update(Money item);
        void Remove(IEnumerable<Money> items);
        void SaveChanges();
    }
}