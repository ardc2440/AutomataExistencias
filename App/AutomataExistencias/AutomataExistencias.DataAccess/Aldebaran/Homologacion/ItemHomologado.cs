using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutomataExistencias.DataAccess.Aldebaran.Homologacion
{    
    [Table("ITEMS_HOMOLOGADOS_VIEW")]
    public class ItemHomologado
    {
        [Key]
        [Column("ITEM_ID")]
        public int ItemId { get; set; }

        [Column("ITEM_ID_HOMOLOGADO")]
        public int ItemIdHomologado { get; set; }

        [Column("LINE_ID_HOMOLOGADA")]
        public int LineIdHomologada { get; set; }

        [Column("HOMOLOGADO")]
        public int Homologado { get; set; }        
    }
}
