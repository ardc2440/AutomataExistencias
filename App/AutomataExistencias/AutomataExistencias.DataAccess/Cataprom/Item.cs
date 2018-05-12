using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Cataprom
{
    [Table("tbl_rItems")]
    public class Item
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("IdItem")]
        public int Id { get; set; }

        [Column("idLinea")]
        public int LineId { get; set; }

        [Column("ItmRefinterna")]
        public string Reference { get; set; }

        [Column("ItmNombre")]
        public string Name { get; set; }

        [Column("ItmRefproveedor")]
        public string ProviderReference { get; set; }

        [Column("ItmNomitemprov")]
        public string ProviderItemName { get; set; }

        [Column("ItmTipoitem")]
        public string ItemType { get; set; }

        [Column("ItmCostofob")]
        public decimal FobCost { get; set; }

        [Column("IdMoneda")]
        public int MoneyId { get; set; }

        [Column("ItmTipparte")]
        public string PartType { get; set; }

        [Column("ItmDeterminante")]
        public string Determinant { get; set; }

        [Column("ItmObservaciones")]
        public string Observations { get; set; }

        [Column("ItmInventarioext")]
        public string StockExt { get; set; }

        [Column("ItmCostocif")]
        public string CifCost { get; set; }

        [Column("ItmVolumen")]
        public decimal? Volume { get; set; }

        [Column("ItmPeso")]
        public decimal? Weight { get; set; }

        [Column("IdUnidadfob")]
        public int? FobUnitId { get; set; }

        [Column("IdUnidadcif")]
        public int? CifUnitId { get; set; }

        [Column("ItmProdnac")]
        public string NationalProduct { get; set; }

        [Column("ItmActivo")]
        public string Active { get; set; }

        [Column("ItmCatalogoVisible")]
        public string VisibleCatalog { get; set; }
    }
}