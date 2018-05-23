using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("REMBALAJES")]
    public class Packaging
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("IDEMBALAJE")]
        public int PackagingId { get; set; }

        [Column("IDITEM")]
        public int? ItemId { get; set; }

        [Column("PESO")]
        public decimal? Weight { get; set; }

        [Column("ALTURA")]
        public decimal? Height { get; set; }

        [Column("ANCHO")]
        public decimal? Width { get; set; }

        [Column("LARGO")]
        public decimal? Long { get; set; }

        [Column("CANTIDAD")]
        public string Quantity { get; set; }

        [Column("ACCION")]
        public string Action { get; set; }

        [Column("INTENTOS")]
        public int Attempts { get; set; }

        [Column("ERROR")]
        public string Exception { get; set; }
    }
}