using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Cataprom
{
    [Table("tbl_sTransito")]
    public class TransitOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("IDITEMTRANSITO")]
        public int Id { get; set; }
        [Column("FECHAESTRECIBO")]
        public DateTime DeliveredDate { get; set; }
        [Column("CANTIDADREC")]
        public int DeliveredQuantity { get; set; }
        [Column("FECHA")]
        public DateTime Date { get; set; }
        [Column("ACTIVIDAD")]
        public string Activity { get; set; }
        [Column("IDITEMXCOLOR")]
        public int ColorItemId { get; set; }
    }
}
