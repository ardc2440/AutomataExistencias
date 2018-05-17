using System.Data.Entity;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.DataAccess.Aldebaran;
using FirebirdSql.Data.FirebirdClient;

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

        #endregion

        #region Constructors
        public AldebaranBaseContext(IAldebaranApplicationEnvironment applicationEnvironment)
            : base(new FbConnection(applicationEnvironment.GetConnectionString()),true)
        {
        }

        #endregion
    }
}