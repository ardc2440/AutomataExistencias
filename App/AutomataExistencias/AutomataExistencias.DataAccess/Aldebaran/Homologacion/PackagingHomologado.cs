using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutomataExistencias.DataAccess.Aldebaran.Homologacion
{    
    [Table("PACKAGING_HOMOLOGADOS_VIEW")]
    public class PackagingHomologado
    {
        [Key]
        [Column("PACKAGING_ID")]
        public short PackagingId { get; set; }

        [Column("PACKAGING_ID_HOMOLOGADO")]
        public int PackagingIdHomologado { get; set; }

        [Column("ITEM_ID_HOMOLOGADO")]
        public int ItemIdHomologado { get; set; }

        [Column("HOMOLOGADO")]
        public int Homologado { get; set; }        
    }
}
