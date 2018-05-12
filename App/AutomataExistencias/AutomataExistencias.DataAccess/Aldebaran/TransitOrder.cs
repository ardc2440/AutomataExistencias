using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("STRANSITO")]
    public class TransitOrder
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Column("IDITEMTRANSITO")]
        public int TransitOrderItemId { get; set; }

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

        [Column("ACCION")]
        public string Action { get; set; }
    }
}