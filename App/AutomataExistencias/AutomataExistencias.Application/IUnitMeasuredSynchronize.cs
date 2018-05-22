using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Application
{
    public interface IUnitMeasuredSynchronize
    {
        void Sync(IEnumerable<UnitMeasured> data, int syncAttempts);
        void ReverseSync(IEnumerable<UnitMeasured> data, int syncAttempts);
    }
}