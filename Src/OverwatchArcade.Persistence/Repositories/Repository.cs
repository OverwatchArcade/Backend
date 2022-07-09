using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OverwatchArcade.Persistence.Repositories.Interfaces;

namespace OverwatchArcade.Persistence.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _mDbSet;
        protected readonly IUnitOfWork MUnitOfWork;

        public Repository(IUnitOfWork unitOfWork)
        {
            MUnitOfWork = unitOfWork;
            _mDbSet = MUnitOfWork.Context.Set<TEntity>();
        }
        
        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _mDbSet.ToListAsync();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _mDbSet.Where(predicate);
        }
        
        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return _mDbSet.Any(predicate);
        }

        public async Task<TEntity?> FirstOrDefaultASync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _mDbSet.FirstOrDefaultAsync(predicate);
        }
        
        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _mDbSet.FirstOrDefault(predicate);
        }

        public void Add(TEntity entity)
        {
            _mDbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _mDbSet.AddRangeAsync(entities);
        }

        public void Remove(TEntity entity)
        {
            _mDbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _mDbSet.RemoveRange(entities);
        }
    }
}
