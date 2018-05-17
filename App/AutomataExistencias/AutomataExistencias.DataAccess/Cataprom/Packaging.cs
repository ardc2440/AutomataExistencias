using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Cataprom
{
    [Table("tbl_rEmbalajes")]
    public class Packaging
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("idEmbalaje")]
        public int Id { get; set; }
        [Column("IdItem")]
        public int? ItemId { get; set; }
        [Column("peso")]
        public decimal? Weight { get; set; }
        [Column("altura")]
        public decimal? Height { get; set; }
        [Column("ancho")]
        public decimal? Width { get; set; }
        [Column("largo")]
        public decimal? Long { get; set; }
        [Column("cantidadCaja")]
        public string Quantity { get; set; }
    }
}