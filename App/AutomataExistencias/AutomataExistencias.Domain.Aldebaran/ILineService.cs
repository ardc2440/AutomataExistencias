using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface ILineService
    {
        IEnumerable<Line> Get();
        void Remove(IEnumerable<Line> entities);
        void SaveChanges();
    }
}