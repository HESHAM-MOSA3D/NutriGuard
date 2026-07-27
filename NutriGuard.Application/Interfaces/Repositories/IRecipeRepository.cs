using NutriGuard.Domain.Entities;

namespace NutriGuard.Application.Interfaces.Repositories;

public interface IRecipeRepository : IGenericRepository<Recipe>
{
    Task<(List<Recipe> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Recipe?> GetDetailsByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}