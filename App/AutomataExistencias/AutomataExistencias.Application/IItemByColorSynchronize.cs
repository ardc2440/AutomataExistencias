using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Application
{
    public interface IItemByColorSynchronize
    {
        void Sync(IEnumerable<ItemByColor> data, int syncAttempts);
        void ReverseSync(IEnumerable<ItemByColor> data, int syncAttempts);
    }
}