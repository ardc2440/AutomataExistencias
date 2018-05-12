using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("RMONEDAS")]
    public class Money
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("IDMONEDA")]
        public int MoneyId { get; set; }
        [Column("NOMMONEDA")]
        public string Name { get; set; }
        [Column("ACCION")]
        public string Action { get; set; }
    }
}
