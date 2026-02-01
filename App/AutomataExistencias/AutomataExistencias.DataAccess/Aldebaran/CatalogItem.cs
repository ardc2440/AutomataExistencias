using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("ITEMS")]
    public class CatalogItem
    {
        [Key]
        [Column("ITEM_ID")]
        public int ItemId { get; set; }

        [Column("LINE_ID")]
        public short? LineId { get; set; }

        [Column("INTERNAL_REFERENCE")]
        public string InternalReference { get; set; }

        [Column("ITEM_NAME")]
        public string Name { get; set; }

        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; }

        [Column("IS_CATALOG_VISIBLE")]
        public bool IsCatalogVisible { get; set; }
    }
}
