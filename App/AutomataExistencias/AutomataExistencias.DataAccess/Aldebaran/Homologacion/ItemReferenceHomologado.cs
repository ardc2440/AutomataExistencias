using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutomataExistencias.DataAccess.Aldebaran.Homologacion
{    
    [Table("ITEM_REFERENCES_HOMOLOGADOS_VIEW")]
    public class ItemReferenceHomologado
    {        
        [Column("ITEM_ID")]
        public int ItemId { get; set; }

        [Column("ITEM_ID_HOMOLOGADO")]
        public int ItemIdHomologado { get; set; }
        
        [Key]
        [Column("REFERENCE_ID")]
        public int ReferenceId { get; set; }

        [Column("REFERENCE_ID_HOMOLOGADO")]
        public int ReferenceIdHomologado { get; set; }

        [Column("REFERENCE_CODE")]
        public string ReferenceCode { get; set; }

        [Column("HOMOLOGADO")]
        public int Homologado { get; set; }        
    }
}