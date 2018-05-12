using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Cataprom
{
    [Table("tbl_rUnidadesMedida")]
    public  class UnitMeasured
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("IdUnidadMedida")]
        public int Id { get; set; }
        [Column("UniNombre")]
        public string Name { get; set; }
        [Column("UniActivo")]
        public string Active { get; set; }
    }
}
