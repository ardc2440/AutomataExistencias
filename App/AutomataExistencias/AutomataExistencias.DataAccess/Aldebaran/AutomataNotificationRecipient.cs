using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("AUTOMATA_NOTIFICATION_RECIPIENTS")]
    public class AutomataNotificationRecipient
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Column("EMAIL")]
        public string Email { get; set; }

        [Column("NOTIFICATIONTYPE")]
        public string NotificationType { get; set; }

        [Column("ISACTIVE")]
        public bool IsActive { get; set; }

        [Column("CREATEDAT")]
        public DateTime CreatedAt { get; set; }

        [Column("UPDATEDAT")]
        public DateTime? UpdatedAt { get; set; }

        [Column("NOTES")]
        public string Notes { get; set; }
        
        [NotMapped]
        public string NotificationTypeUpper => NotificationType?.ToUpperInvariant();
    }
}
