using System.Data.Entity;

namespace AutomataExistencias.DataAccess.Core.Contract
{
    public interface IUnitOfWork
    {
        void SaveChanges();
        DbContext GetContext();
        IRepository<T> Repository<T>() where T : class;
        void SetAutoDetectChanges(bool enabled);
    }

    public interface IUnitOfWorkAldebaran : IUnitOfWork { }
    public interface IUnitOfWorkCataprom : IUnitOfWork { }
}
