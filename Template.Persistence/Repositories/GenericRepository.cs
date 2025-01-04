using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Template.Application.Contracts.Persistence;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Template.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly TemplateDbContext _dbContext;

        public GenericRepository(TemplateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.AddAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public async Task<bool> Exists(int id)
        {
            T entity = await GetAsync(id);
            return entity != null;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includeExpressions)
        {
            var query = _dbContext.Set<T>().AsQueryable();

            foreach (var includeExpression in includeExpressions)
            {
                query = query.Include(includeExpression);
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }


        public async Task<T> GetAsync(long id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            var entityType = _dbContext.Model.FindEntityType(typeof(T));
            var primaryKey = entityType?.FindPrimaryKey();
            if (primaryKey == null)
                throw new InvalidOperationException("The entity does not have a primary key defined.");

            var keyValues = primaryKey.Properties
                                       .Select(p => p.PropertyInfo.GetValue(entity))
                                       .ToArray();

            if (keyValues.Any(kv => kv == null))
                throw new ArgumentException("Primary key value(s) cannot be null.");

            var existingEntity = await _dbContext.Set<T>().FindAsync(keyValues);
            if (existingEntity != null)
            {
                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                _dbContext.Set<T>().Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Modified;
            }

            await _dbContext.SaveChangesAsync();
        }

    }
}
