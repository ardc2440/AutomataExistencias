using System.Data.Entity;
using AutomataExistencias.Core.Configuration;
using AutomataExistencias.DataAccess.Aldebaran;
using FirebirdSql.Data.FirebirdClient;

namespace AutomataExistencias.DataAccess.Core
{
    public class AldebaranBaseContext : DbContext
    {
        #region Properties

        public DbSet<Balance> Balance { get; set; }

        #endregion

        #region Constructors
        public AldebaranBaseContext(IAldebaranApplicationEnvironment applicationEnvironment)
            : base(new FbConnection(applicationEnvironment.GetConnectionString()),true)
        {
        }

        #endregion
    }
}