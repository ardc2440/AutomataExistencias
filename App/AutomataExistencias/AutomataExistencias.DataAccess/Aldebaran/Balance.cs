using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("INTEGRASALDOS")]
    public class Balance
    {
        [Key]
        [Column("IDITEMXCOLOR", Order = 0)]
        public int ColorItemId { get; set; }
        [Key]
        [Column("IDBODEGA", Order = 1)]
        public int StorageCellarId { get; set; }

        [Column("IDITEM")]
        public int ItemId { get; set; }
        [Column("BODEGA")]
        public string StorageCellar { get; set; }
        [Column("CANTIDAD")]
        public int Quantity { get; set; }
        [Column("COLOR")]
        public string Color { get; set; }
        [Column("ACCION")]
        public string Action { get; set; }
    }
}
