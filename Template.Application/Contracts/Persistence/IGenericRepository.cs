using System.Linq.Expressions;

namespace Template.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetAsync(long id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> Exists(int id);
        Task<IReadOnlyList<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includeExpressions);
    }
}
