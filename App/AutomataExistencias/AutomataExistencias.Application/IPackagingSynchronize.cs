using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Application
{
    public interface IPackagingSynchronize
    {
        void Sync(IEnumerable<Packaging> data, int syncAttempts);
        void ReverseSync(IEnumerable<Packaging> data, int syncAttempts);
    }
}