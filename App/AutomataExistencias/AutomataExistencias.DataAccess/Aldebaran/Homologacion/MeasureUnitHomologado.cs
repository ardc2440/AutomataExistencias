using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutomataExistencias.DataAccess.Aldebaran.Homologacion
{    
    [Table("MEASURE_UNITS_HOMOLOGADOS_VIEW")]
    public class MeasureUnitHomologado
    {
        [Key]
        [Column("MEASURE_UNIT_ID")]
        public short MeasureUnitId { get; set; }

        [Column("MEASURE_UNIT_ID_HOMOLOGADO")]
        public int MeasureUnitIdHomologado { get; set; }

        [Column("HOMOLOGADO")]
        public int Homologado { get; set; }        
    }
}
