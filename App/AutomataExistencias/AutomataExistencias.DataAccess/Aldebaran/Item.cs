﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("RITEMS")]
    public class Item
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Column("IDITEM")]
        public int ItemId { get; set; }

        [Column("IDLINEA")]
        public short? LineId { get; set; }

        [Column("REFINTERNA")]
        public string Reference { get; set; }

        [Column("NOMITEM")]
        public string Name { get; set; }

        [Column("REFPROVEEDOR")]
        public string ProviderReference { get; set; }

        [Column("NOMITEMPROV")]
        public string ProviderItemName { get; set; }

        [Column("TIPOITEM")]
        public string ItemType { get; set; }

        [Column("COSTOFOB")]
        public double? FobCost { get; set; }

        [Column("IDMONEDA")]
        public short? MoneyId { get; set; }

        [Column("TIPPARTE")]
        public string PartType { get; set; }

        [Column("DETERMINANTE")]
        public string Determinant { get; set; }

        [Column("OBSERVACIONES")]
        public string Observations { get; set; }

        [Column("INVENTARIOEXT")]
        public string StockExt { get; set; }

        [Column("COSTOCIF")]
        public string CifCost { get; set; }

        [Column("VOLUMEN")]
        public double? Volume { get; set; }

        [Column("PESO")]
        public double? Weight { get; set; }

        [Column("IDUNIDADFOB")]
        public short? FobUnitId { get; set; }

        [Column("IDUNIDADCIF")]
        public short? CifUnitId { get; set; }

        [Column("PRODNAC")]
        public string NationalProduct { get; set; }

        [Column("ACTIVO")]
        public string Active { get; set; }

        [Column("CATALOGOVISIBLE")]
        public string VisibleCatalog { get; set; }

        [Column("ACCION")]
        public string Action { get; set; }

        [Column("INTENTOS")]
        public int Attempts { get; set; }

        [Column("ERROR")]
        public string Exception { get; set; }
    }
}