using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using OverwatchArcade.Persistence.Persistence;
using OverwatchArcade.Persistence.Persistence.Repositories.Interfaces;

namespace OWArcadeBackend.Persistence.Repositories
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

        public async Task<TEntity> Get(int id)
        {
            return await _mDbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _mDbSet.ToListAsync();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _mDbSet.Where(predicate);
        }

        public TEntity GetBy(Expression<Func<TEntity, bool>> predicate)
        {
            var results = _mDbSet.Where(predicate);
            if (results.Count() > 1)
            {
                throw new Exception("More than 1 result found");
            }

            return results.First();
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return _mDbSet.Any(predicate);
        }

        public async Task<TEntity> SingleOrDefaultASync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _mDbSet.SingleOrDefaultAsync(predicate);
        }
        
        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _mDbSet.SingleOrDefault(predicate);
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
