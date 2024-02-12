using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("REXISTENCIAS")]
    public class Stock
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Column("IDITEMXCOLOR")]
        public short ColorItemId { get; set; }

        [Column("IDITEM")]
        public short ItemId { get; set; }

        [Column("IDBODEGA")]
        public short StorageCellarId { get; set; }

        [Column("COLOR")]
        public string Color { get; set; }

        [Column("CANTIDAD")]
        public int Quantity { get; set; }

        [Column("BODEGA")]
        public string StorageCellar { get; set; }

        [Column("ACCION")]
        public string Action { get; set; }

        [Column("INTENTOS")]
        public int Attempts { get; set; }

        [Column("ERROR")]
        public string Exception { get; set; }
    }
}