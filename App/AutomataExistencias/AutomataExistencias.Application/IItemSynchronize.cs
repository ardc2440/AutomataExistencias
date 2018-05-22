using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Application
{
    public interface IItemSynchronize
    {
        void Sync(IEnumerable<Item> data, int syncAttempts);
        void ReverseSync(IEnumerable<Item> data, int syncAttempts);
    }
}