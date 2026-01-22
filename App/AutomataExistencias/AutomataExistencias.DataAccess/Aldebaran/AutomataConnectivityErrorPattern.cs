using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutomataExistencias.DataAccess.Aldebaran
{
    [Table("AUTOMATA_CONNECTIVITY_ERROR_PATTERNS")]
    public class AutomataConnectivityErrorPattern
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int Id { get; set; }

        [Column("PATTERN")]
        public string Pattern { get; set; }

        [Column("TARGET")]
        public string Target { get; set; }

        // Convenience: uppercase pattern for fast comparisons
        [NotMapped]
        public string PatternUpper => Pattern?.ToUpperInvariant();

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
