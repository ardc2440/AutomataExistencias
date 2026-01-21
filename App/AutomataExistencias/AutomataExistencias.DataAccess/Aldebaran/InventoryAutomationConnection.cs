using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("Inventory_Automation_Connections")]
    public class InventoryAutomationConnection
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("INVENTORY_AUTOMATION_CONNECTION_ID")]
        public int InventoryAutomationConnectionId { get; set; }

        [Column("SERVER_NAME")]
        public string ServerName { get; set; }

        [Column("PORT_NUMBER")]
        public string PortNumber { get; set; }

        [Column("DATABASE_NAME")]
        public string DatabaseName { get; set; }

        [Column("USER_ID")]
        public string UserId { get; set; }

        [Column("PASSWORD")]
        public string Password { get; set; }

        [Column("ACTIVE")]
        public bool Active { get; set; }
    }
}
