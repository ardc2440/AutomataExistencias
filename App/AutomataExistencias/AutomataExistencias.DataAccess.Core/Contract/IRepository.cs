using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AutomataExistencias.DataAccess.Core.Contract
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void Remove(IEnumerable<T> entities);
        T Find<TK>(TK id);
        IEnumerable<T> Get();
        IEnumerable<T> Get(Expression<Func<T, bool>> where);
        IEnumerable<T> GetWithoutNoTracking(Expression<Func<T, bool>> where);
        IEnumerable<T> Get(params Expression<Func<T, object>>[] includes);
        IEnumerable<T> Get(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] includes);
        T GetByWhere(Expression<Func<T, bool>> where);
        void SaveChanges();
    }
}