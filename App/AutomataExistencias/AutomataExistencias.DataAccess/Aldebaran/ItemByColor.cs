using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("RITEMSXCOLOR")]
    public class ItemByColor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }
        [Column("IDITEMXCOLOR")]
        public int ColorItemId { get; set; }
        [Column("IDITEM")]
        public int ItemId { get; set; }
        [Column("REFITEMXCOLOR")]
        public string ItemByColorReference { get; set; }
        [Column("REFINTITEMXCOLOR")]
        public string ItemByColorInternalReference { get; set; }
        [Column("NOMCOLOR")]
        public string ColorName { get; set; }
        [Column("NOMITEMXCOLORPROV")]
        public string ProviderNomItemByColor { get; set; }
        [Column("OBSERVACIONES")]
        public string Observations { get; set; }
        [Column("COLOR")]
        public string Color { get; set; }
        [Column("CANTPEDIDA")]
        public int QuantityOrder { get; set; }
        [Column("CANTIDAD")]
        public int Quantity { get; set; }
        [Column("CANTRESERVADA")]
        public int QuantityReserved { get; set; }
        [Column("CANTPEDIDAPAN")]
        public int QuantityOrderPan { get; set; }
        [Column("CANTIDADPAN")]
        public int QuantityPan { get; set; }
        [Column("CANTRESERVADAPAN")]
        public int QuantityReservedPan { get; set; }
        [Column("ACTIVO")]
        public string Active { get; set; }
        [Column("AGOTADO")]
        public string SoldOut { get; set; }
        [Column("CANTPROCESO")]
        public int QuantityProcess { get; set; }
        [Column("ACCION")]
        public string Action { get; set; }
    }
}
