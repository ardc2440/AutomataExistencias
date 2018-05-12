using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Cataprom
{
    [Table("tbl_rMonedas")]
    public class Money
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("IdMoneda")]
        public int Id { get; set; }
        [Column("MonNombre")]
        public string Name { get; set; }
        [Column("MonActivo")]
        public string Active { get; set; }
    }
}
