using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Cataprom
{
    [Table("tbl_rLineas")]
    public class Line
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("idLinea")]
        public int Id { get; set; }
        [Column("LinCodigo")]
        public string Code { get; set; }
        [Column("LinNombre")]
        public string Name { get; set; }
        [Column("LinDemonio")]
        public string Daemon { get; set; }
        [Column("LinActivo")]
        public string Active { get; set; }
    }
}
