using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Cataprom
{
    [Table("tbl_rexistencias")]
    public class Stock
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("IBASE_IDITEMEXISTENCIAS")]
        public int Id { get; set; }
    }
}
