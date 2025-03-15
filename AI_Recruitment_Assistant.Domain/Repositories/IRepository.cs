
using System.Linq.Expressions;

namespace AI_Recruitment_Assistant.Domain.Repositories;

public interface IRepository
{
    Task<T> CreateAsync<T>(T entity, CancellationToken cancellationToken) where T : class;
    Task<T> UpdateAsync<T>(T entity, CancellationToken cancellationToken) where T : class;
    Task<bool> DeleteAsync<T>(T entity, CancellationToken cancellationToken) where T : class;
    Task<List<T>> GetAllAsync<T>(int? cursor = null, int? pageSize = null, CancellationToken cancellationToken = default) where T : class;
    Task<T?> GetByIdAsync<T>(int id, CancellationToken cancellationToken) where T : class;
    Task<List<T>> FindByConditionAsync<T>(Expression<Func<T, bool>> predicate, int? cursor = null, int? pageSize = null, CancellationToken cancellationToken = default) where T : class;
}
