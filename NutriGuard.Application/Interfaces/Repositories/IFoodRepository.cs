using NutriGuard.Domain.Entities;

namespace NutriGuard.Application.Interfaces.Repositories;

public interface IFoodRepository : IGenericRepository<Food>
{
    Task<IReadOnlyList<Food>> SearchAsync(
        string query,
        CancellationToken cancellationToken = default);
}