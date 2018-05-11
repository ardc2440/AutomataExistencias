using System;
using System.Collections.Generic;
using System.Data.Entity;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.DataAccess.Core
{
    public class UnitOfWorkCataprom : IUnitOfWorkCataprom
    {
        public UnitOfWorkCataprom(CatapromBaseContext context)
        {
            _context = context;
            _repositories = new Dictionary<string, object>();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public DbContext GetContext()
        {
            return _context;
        }

        // ReSharper disable once InconsistentNaming
        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof (T).Name;

            if (_repositories.ContainsKey(type))
                return (Repository<T>) _repositories[type];

            var repositoryType = typeof (Repository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof (T)), _context);
            _repositories.Add(type, repositoryInstance);

            return (Repository<T>) _repositories[type];
        }

        public void SetAutoDetectChanges(bool enabled)
        {
            _context.Configuration.AutoDetectChangesEnabled = enabled;
        }

        protected void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();
            _disposed = true;
        }

        #region Private Fields

        private readonly Dictionary<string, object> _repositories;
        private readonly CatapromBaseContext _context;
        private bool _disposed;

        #endregion Private Fields
    }
}