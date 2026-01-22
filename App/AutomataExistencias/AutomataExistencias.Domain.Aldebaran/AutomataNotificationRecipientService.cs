using System.Collections.Generic;
using System.Linq;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.Domain.Aldebaran
{
    public class AutomataNotificationRecipientService : IAutomataNotificationRecipientService
    {
        private readonly IUnitOfWorkAldebaran _unitOfWork;

        public AutomataNotificationRecipientService(IUnitOfWorkAldebaran unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<AutomataNotificationRecipient> GetActiveByType(string notificationType)
        {
            return _unitOfWork.Repository<AutomataNotificationRecipient>()
                .Get(w => w.IsActive && w.NotificationType.ToUpper() == notificationType.ToUpper());
        }
    }
}
