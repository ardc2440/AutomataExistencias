using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Cataprom
{
    [Table("tbl_rItemsXColor")]
    public class ItemByColor
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("IdItemxcolor")]
        public int Id { get; set; }
        [Column("IdItem")]
        public int ItemId { get; set; }
        [Column("ItcRefitemxcolor")]
        public string ItemByColorReference { get; set; }
        [Column("ItcRefintitemxcolor")]
        public string ItemByColorInternalReference { get; set; }
        [Column("ItcNombre")]
        public string ColorName { get; set; }
        [Column("ItcNomitemxcolorprov")]
        public string ProviderNomItemByColor { get; set; }
        [Column("ItcObservaciones")]
        public string Observations { get; set; }
        [Column("ItcColor")]
        public string Color { get; set; }
        [Column("ItcCantpedida")]
        public int QuantityOrder { get; set; }
        [Column("ItcCantidad")]
        public int Quantity { get; set; }
        [Column("ItcCantreservada")]
        public int QuantityReserved { get; set; }
        [Column("ItcCantpedidapan")]
        public int QuantityOrderPan { get; set; }
        [Column("ItcCantidadpan")]
        public int QuantityPan { get; set; }
        [Column("ItcCantreservadapan")]
        public int QuantityReservedPan { get; set; }
        [Column("ItcActivo")]
        public string Active { get; set; }
        [Column("ItcAgotado")]
        public string SoldOut { get; set; }
        [Column("ItcCantproceso")]
        public int QuantityProcess { get; set; }
    }
}
