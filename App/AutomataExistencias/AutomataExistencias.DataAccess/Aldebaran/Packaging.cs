using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("REMBALAJES")]
    public class Packaging
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Column("IDEMBALAJE")]
        public short PackagingId { get; set; }

        [Column("IDITEM")]
        public short? ItemId { get; set; }

        [Column("PESO")]
        public double? Weight { get; set; }

        [Column("ALTURA")]
        public double? Height { get; set; }

        [Column("ANCHO")]
        public double? Width { get; set; }

        [Column("LARGO")]
        public double? Long { get; set; }

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