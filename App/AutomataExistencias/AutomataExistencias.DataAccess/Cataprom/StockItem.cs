using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Cataprom
{
    [Table("IBASE_ITEMEXISTENCIAS")]
    public class StockItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("IBASE_IDITEMEXISTENCIAS")]
        public int Id { get; set; }

        [Column("IDITEMXCOLOR")]
        public int ColorItemId { get; set; }

        [Column("IDITEM")]
        public int ItemId { get; set; }

        [Column("COLOR")]
        public string Color { get; set; }

        [Column("CANTIDAD")]
        public int Quantity { get; set; }

        [Column("BODEGA")]
        public string StorageCellar { get; set; }
    }
}