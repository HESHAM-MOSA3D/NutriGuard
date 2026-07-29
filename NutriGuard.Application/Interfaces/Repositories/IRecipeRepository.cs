using NutriGuard.Domain.Entities;

namespace NutriGuard.Application.Interfaces.Repositories;

public interface IRecipeRepository : IGenericRepository<Recipe>
{
 


    Task<Recipe?> GetDetailsByIdAsync(
        int id,
        CancellationToken cancellationToken = default);


 

    Task<IReadOnlyList<Recipe>> GetAllAsync(
    CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Recipe>, int)> SearchAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken = default);
}