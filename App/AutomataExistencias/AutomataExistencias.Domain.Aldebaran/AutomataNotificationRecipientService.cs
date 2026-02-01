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
            // Support legacy logical types used in code by mapping them
            // to the concrete database values enforced by the DB CHECK constraint
            if (string.Equals(notificationType, "CONNECTIVITY", System.StringComparison.OrdinalIgnoreCase))
            {
                return _unitOfWork.Repository<AutomataNotificationRecipient>()
                    .Get(w => w.IsActive && (w.NotificationTypeUpper == "CONNECTIVITY_DOWN" || w.NotificationTypeUpper == "CONNECTIVITY_RECOVERED"));
            }

            // map legacy/general notifications to BUSINESS_ERROR (DB value: 'BUSINESS_ERROR')
            if (string.Equals(notificationType, "GENERAL", System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(notificationType, "BUSINESS", System.StringComparison.OrdinalIgnoreCase))
            {
                return _unitOfWork.Repository<AutomataNotificationRecipient>()
                    .Get(w => w.IsActive && w.NotificationTypeUpper == "BUSINESS_ERROR");
            }

            return _unitOfWork.Repository<AutomataNotificationRecipient>()
                .Get(w => w.IsActive && w.NotificationType.ToUpper() == notificationType.ToUpper());
        }
    }
}
