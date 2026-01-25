using System.Collections.Generic;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IRecoveryDomainService
    {
        IEnumerable<int> GetCandidateItemIds(int syncAttempts);
        void MarkEventsAsFlagged(int itemId, int flagAttempts);
        int CountPendingEvents(int itemId, int syncAttempts);
        void ClearEventsForItem(int itemId);
    }
}
