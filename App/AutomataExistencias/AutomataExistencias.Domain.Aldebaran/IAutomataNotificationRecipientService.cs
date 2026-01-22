using System.Collections.Generic;
using AutomataExistencias.DataAccess.Aldebaran;

namespace AutomataExistencias.Domain.Aldebaran
{
    public interface IAutomataNotificationRecipientService
    {
        IEnumerable<AutomataNotificationRecipient> GetActiveByType(string notificationType);
    }
}
