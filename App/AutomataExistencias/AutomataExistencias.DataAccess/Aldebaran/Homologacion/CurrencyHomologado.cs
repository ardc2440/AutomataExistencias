using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace AutomataExistencias.DataAccess.Aldebaran.Homologacion
{    
    [Table("CURRENCIES_HOMOLOGADOS_VIEW")]
    public class CurrencyHomologado
    {
        [Key]
        [Column("CURRENCY_ID")]
        public short CurrencyId { get; set; }

        [Column("CURRENCY_ID_HOMOLOGADO")]
        public int CurrencyIdHomologado { get; set; }

        [Column("HOMOLOGADO")]
        public int Homologado { get; set; }        
    }
}
