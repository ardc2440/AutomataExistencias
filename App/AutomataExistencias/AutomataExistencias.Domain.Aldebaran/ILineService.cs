using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface ILineService
    {
        IEnumerable<Line> Get();
        IEnumerable<Line> Get(int attempts);
        void Remove(Line item);
        void Update(Line item);
        void Remove(IEnumerable<Line> items);
        void SaveChanges();
    }
}