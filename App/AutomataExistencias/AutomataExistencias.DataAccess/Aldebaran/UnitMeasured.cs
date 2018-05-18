using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("RUNIDADESMEDIDA")]
    public class UnitMeasured
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("IDUNIDAD")]
        public int UnitMeasuredId { get; set; }
        [Column("NOMUNIDAD")]
        public string Name { get; set; }
        [Column("ACCION")]
        public string Action { get; set; }

        [Column("INTENTOS")]
        public int Attempts { get; set; }
        [Column("ERROR")]
        public string Exception { get; set; }
    }
}
