using System.Data.Entity;
using System.Data.SqlClient;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.DataAccess.Aldebaran;
using AutomataExistencias.DataAccess.Aldebaran.Homologacion;

namespace AutomataExistencias.DataAccess.Core
{
    public class AldebaranBaseContext : DbContext
    {
        #region Properties
        public DbSet<Stock> Stock { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<ItemByColor> ItemByColor { get; set; }
        public DbSet<Line> Line { get; set; }
        public DbSet<Money> Money { get; set; }
        public DbSet<UnitMeasured> UnitMeasured { get; set; }
        public DbSet<TransitOrder> TransitOrder { get; set; }
        public DbSet<Packaging> Packaging { get; set; }

        public DbSet<ItemHomologado> ItemHomologados { get; set; }
        public DbSet<ItemReferenceHomologado> ItemReferenceHomologados { get; set; }
        public DbSet<CurrencyHomologado> CurrencyHomologados { get; set; }
        public DbSet<MeasureUnitHomologado> MeasureUnitHomologados { get; set; }
        public DbSet<PackagingHomologado> PackagingHomologados { get; set; }

        #endregion

        #region Constructors
        public AldebaranBaseContext(IAldebaranApplicationEnvironment applicationEnvironment)
            : base(new SqlConnection(applicationEnvironment.GetConnectionString()), true)
        {
        }

        #endregion
    }
}