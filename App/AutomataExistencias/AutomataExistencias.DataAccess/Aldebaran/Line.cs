using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("RLINEAS")]
    public class Line
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Column("IDLINEA")]
        public short LineId { get; set; }

        [Column("CODLINEA")]
        public string Code { get; set; }

        [Column("NOMLINEA")]
        public string Name { get; set; }

        [Column("DEMONIO")]
        public string Daemon { get; set; }

        [Column("ACTIVO")]
        public string Active { get; set; }

        [Column("ACCION")]
        public string Action { get; set; }

        [Column("INTENTOS")]
        public int Attempts { get; set; }

        [Column("ERROR")]
        public string Exception { get; set; }
    }
}