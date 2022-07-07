using System.Linq.Expressions;

namespace OverwatchArcade.Persistence.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        bool Exists(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> FirstOrDefaultASync(Expression<Func<TEntity, bool>> predicate);
        TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
