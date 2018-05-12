using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("REXISTENCIAS")]
    public class Stock
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("IDITEMXCOLOR")]
        public int ColorItemId { get; set; }

        [Column("IDITEM")]
        public int ItemId { get; set; }

        [Column("IDBODEGA")]
        public int StorageCellarId { get; set; }

        [Column("COLOR")]
        public string Color { get; set; }

        [Column("CANTIDAD")]
        public int Quantity { get; set; }

        [Column("BODEGA")]
        public string StorageCellar { get; set; }

        [Column("ACCION")]
        public string Action { get; set; }
    }
}