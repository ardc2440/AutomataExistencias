using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Application
{
    public interface IMoneySynchronize
    {
        void Sync(IEnumerable<Money> data, int syncAttempts);
        void ReverseSync(IEnumerable<Money> data, int syncAttempts);
    }
}