using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using AutomataExistencias.DataAccess.Core.Contract;

namespace AutomataExistencias.DataAccess.Core
{
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Constructors

        public Repository(DbContext context)
        {
            _context = context;
            _entitySet = context.Set<T>();
        }

        #endregion

        #region Private Fields

        private readonly IDbSet<T> _entitySet;
        private readonly DbContext _context;

        #endregion Private Fields

        #region Public Persistence Methods

        public void Add(T entity)
        {
            _context.Entry(entity).State = EntityState.Added;
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            _context.Entry<T>(entity).State = EntityState.Deleted;
        }
        public void Remove(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }
        }
        #endregion

        #region Public Read Methods

        public T Find<TK>(TK id)
        {
            return _entitySet.Find(id);
        }

        public IEnumerable<T> Get()
        {
            return _entitySet.AsNoTracking();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> where)
        {
            return _entitySet.Where(where).AsNoTracking();
        }

        public IEnumerable<T> GetWithoutNoTracking(Expression<Func<T, bool>> where)
        {
            return _entitySet.Where(where);
        }

        public T GetByWhere(Expression<Func<T, bool>> where)
        {
            return _entitySet.Where(where).FirstOrDefault();
        }

        public IEnumerable<T> Get(params Expression<Func<T, object>>[] includes)
        {
            var query = _entitySet.AsNoTracking();

            return includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes)
        {
            var query = _entitySet.Where(where).AsNoTracking();
            return includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }
        
        #endregion
    }
}