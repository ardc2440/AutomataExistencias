using System.Collections.Generic;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.Domain.Cataprom
{
    public interface ILineService
    {
        IEnumerable<Line> Get();
        void AddOrUpdate(Line item);
        void Remove(Line item);
        void Remove(List<Line> items);
        void SaveChanges();
    }
}
