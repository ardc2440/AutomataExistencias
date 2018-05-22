using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Application
{
    public interface ILineSynchronize
    {
        void Sync(IEnumerable<Line> data, int syncAttempts);
        void ReverseSync(IEnumerable<Line> data, int syncAttempts);
    }
}