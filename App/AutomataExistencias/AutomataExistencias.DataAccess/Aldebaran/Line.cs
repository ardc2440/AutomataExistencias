using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("RLINEAS")]
    public class Line
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }
        [Column("IDLINEA")]
        public int LineId { get; set; }
        [Column("CODLINEA")]
        public string LineCode { get; set; }
        [Column("NOMLINEA")]
        public string LineName { get; set; }
        [Column("DEMONIO")]
        public string Daemon { get; set; }
        [Column("ACTIVO")]
        public string Active { get; set; }
        [Column("ACCION")]
        public string Action { get; set; }
    }
}
