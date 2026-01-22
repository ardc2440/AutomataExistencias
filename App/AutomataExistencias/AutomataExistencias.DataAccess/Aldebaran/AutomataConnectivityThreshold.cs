using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("AUTOMATA_CONNECTIVITY_THRESHOLDS")]
    public class AutomataConnectivityThreshold
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Column("ENTITYNAME")]
        public string EntityName { get; set; }

        [NotMapped]
        public string EntityNameUpper => EntityName?.ToUpperInvariant();

        [Column("ERRORCOUNTTHRESHOLD")]
        public int ErrorCountThreshold { get; set; }

        [Column("ISACTIVE")]
        public bool IsActive { get; set; }

        [Column("CREATEDAT")]
        public DateTime CreatedAt { get; set; }

        [Column("UPDATEDAT")]
        public DateTime? UpdatedAt { get; set; }

        [Column("NOTES")]
        public string Notes { get; set; }
    }
}
