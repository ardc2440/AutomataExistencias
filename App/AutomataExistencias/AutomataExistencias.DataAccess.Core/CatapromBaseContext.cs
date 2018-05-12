using System.Data.Entity;
using System.Data.SqlClient;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.DataAccess.Cataprom;

namespace AutomataExistencias.DataAccess.Core
{
    public class CatapromBaseContext : DbContext
    {
        #region Properties
        public DbSet<Money> Money { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<ItemByColor> ItemByColor { get; set; }

        #endregion

        #region Constructors

        public class CatapromBaseContextInitializer : CreateDatabaseIfNotExists<CatapromBaseContext>
        {

        }
        public CatapromBaseContext(ICatapromApplicationEnvironment applicationEnvironment)
            : base(new SqlConnection(applicationEnvironment.GetConnectionString()), true)
        {
            Database.SetInitializer<CatapromBaseContext>(new CatapromBaseContextInitializer());
        }

        #endregion
    }
}