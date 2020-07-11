using FameLink.Data.Models;
using FameLink.Infrastructure.Extensions.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FameLink.Infrastructure.Abstractions.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            Context = context;
        }

        public async Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Set().ToListAsync(cancellationToken);
        }

        public async Task<(int count, ICollection<TEntity> items)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            var items = await Set()
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var count = await Set().CountAsync();

            return (count, items);
        }

        public TEntity Get(Guid id)
        {
            return Set().Find(id);
        }

        public async Task<TEntity> GetAsync(Guid id)
        {
            return await Set().FindAsync(id);
        }

        public virtual TEntity Add(TEntity entity)
        {
            Set().Add(entity);
            return entity;
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken token)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            await Set().AddAsync(entity, token);
            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken token)
        {
            if (entities is null) throw new ArgumentNullException(nameof(entities));

            await Set().AddRangeAsync(entities);
            return entities;
        }

        public TEntity Find(Expression<Func<TEntity, bool>> match)
        {
            return Set().SingleOrDefault(match);
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match, CancellationToken cancellationToken = default)
        {
            return await Set().SingleOrDefaultAsync(match, cancellationToken);
        }

        public async Task<TEntity> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Set().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<TEntity> FindAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            var set = Set().AsQueryable();
            if (includes.Any())
            {
                set = includes.Aggregate(set,
                    (current, include) => current.Include(include));
            }
            return await set.FirstOrDefaultAsync(ent => ent.Id == id, cancellationToken);
        }

        public ICollection<TEntity> FindAll(Expression<Func<TEntity, bool>> match)
        {
            return Set().Where(match).ToList();
        }

        public async Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match, CancellationToken cancellationToken = default)
        {
            return await Set().Where(match).ToListAsync(cancellationToken);
        }

        public virtual void Delete(TEntity entity)
        {
            TEntity exist = Set().Find(entity.Id);
            if (exist == null)
                return;

            Set().Remove(exist);
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken token)
        {
            TEntity exist = await Set().FindAsync(entity.Id);
            if (exist == null)
                return;

            Set().Remove(exist);
        }

        public virtual void Delete(IEnumerable<TEntity> entity)
        {
            IEnumerable<Guid> entityIds = entity.Select(e => e.Id);
            IEnumerable<TEntity> items = Set().Where(e => entityIds.Contains(e.Id));
            if (items.IsEmpty())
                return;

            Set().RemoveRange(items);
        }

        public TEntity Update(TEntity entity, object key)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            TEntity exist = Set().Find(key);
            if (exist is null)
                throw new ArgumentException($"Entity with key={key} not found to update", nameof(key));

            UpdateFactoryMethod(entity, exist);

            Context.Entry(exist).CurrentValues.SetValues(entity);

            return exist;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, object key, CancellationToken token)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            TEntity exist = await Set().FindAsync(key);
            if (exist is null)
                throw new ArgumentException($"Entity with key={key} not found to update", nameof(key));

            UpdateFactoryMethod(entity, exist);

            Context.Entry(exist).CurrentValues.SetValues(entity);
            return exist;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken token)
        {
            return await UpdateAsync(entity, entity.Id, token);
        }

        protected virtual void UpdateFactoryMethod(TEntity entity, TEntity existingModel) { }

        public int Count()
        {
            return Set().Count();
        }

        public async Task<int> CountAsync(CancellationToken token)
        {
            return await Set().CountAsync(token);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> filter)
        {
            var entitySet = Set().AsNoTracking();
            var count = entitySet.Count(filter);
            return Task.FromResult(count);
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = Set().Where(predicate);
            return query;
        }

        public async Task<ICollection<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Set().Where(predicate).ToListAsync();
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {

            IQueryable<TEntity> queryable = Set();
            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include<TEntity, object>(includeProperty);
            }
            return queryable;
        }

        public IEnumerable<TEntity> GetAllElements<TKey>(Expression<Func<TEntity, TKey>> orderByExpression,
                                                         Expression<Func<TEntity, bool>> filterExpression,
                                                         bool ascending,
                                                  params Expression<Func<TEntity, object>>[] includes)
        {
            if (orderByExpression == null)
            {
                throw new ArgumentNullException("orderByExpression");
            }

            var entitySet = Set().AsNoTracking();

            var searchedSet = filterExpression == null ? entitySet : entitySet.Where(filterExpression);

            if (ascending)
            {
                if (includes.Any())
                {
                    return includes.Aggregate(searchedSet.OrderBy(orderByExpression).AsQueryable(),
                        (current, include) => current.Include(include));
                }

                return searchedSet.OrderBy(orderByExpression);
            }
            if (includes.Any())
            {
                return includes.Aggregate(searchedSet.OrderBy(orderByExpression).AsQueryable(),
                    (current, include) => current.Include(include));
            }

            return searchedSet.OrderByDescending(orderByExpression);
        }

        public Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter,
                                                    CancellationToken cancellationToken = default)
        {
            var queryable = Set().AsNoTracking();

            var entity = filter == null
                ? queryable.FirstOrDefaultAsync(cancellationToken).Result
                : queryable.FirstOrDefaultAsync(filter, cancellationToken).Result;

            return Task.FromResult(entity);
        }

        private DbSet<TEntity> Set()
        {
            return Context.Set<TEntity>();
        }

        private bool _disposed = false;
        protected void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
                this._disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
