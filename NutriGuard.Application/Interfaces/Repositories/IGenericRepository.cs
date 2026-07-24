using System.Linq.Expressions;

namespace NutriGuard.Application.Interfaces.Repositories;

public interface IGenericRepository<T>
    where T : class
{
    Task<T?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Expression<Func<T, bool>> filter,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default);

    void Update(T entity);

    void Delete(T entity);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}