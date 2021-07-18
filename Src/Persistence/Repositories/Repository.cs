using Microsoft.EntityFrameworkCore;
using OWArcadeBackend.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OWArcadeBackend.Persistence.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbSet<TEntity> mDbSet;

        protected IUnitOfWork mUnitOfWork;

        public Repository(IUnitOfWork unitOfWork)
        {
            mUnitOfWork = unitOfWork;
            mDbSet = mUnitOfWork.Context.Set<TEntity>();
        }

        public async Task<TEntity> Get(int id)
        {
            return await mDbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await mDbSet.ToListAsync();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return mDbSet.Where(predicate);
        }

        public TEntity GetBy(Expression<Func<TEntity, bool>> predicate)
        {
            var results = mDbSet.Where(predicate);
            if (results.Count() > 1)
            {
                throw new Exception("More than 1 result found");
            }

            return results.First();
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return mDbSet.Any(predicate);
        }

        public async Task<TEntity> SingleOrDefaultASync(Expression<Func<TEntity, bool>> predicate)
        {
            return await mDbSet.SingleOrDefaultAsync(predicate);
        }
        
        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return mDbSet.SingleOrDefault(predicate);
        }

        public void Add(TEntity entity)
        {
            mDbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            mDbSet.AddRangeAsync(entities);
        }

        public void Remove(TEntity entity)
        {
            mDbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            mDbSet.RemoveRange(entities);
        }
    }
}
