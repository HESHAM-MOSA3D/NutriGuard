using NutriGuard.Domain.Entities;

namespace NutriGuard.Application.Interfaces.Repositories;

public interface IFoodRepository : IGenericRepository<Food>
{
    Task<IReadOnlyList<Food>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Food?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Food>> GetByCategoryAsync(
        int categoryId,
        CancellationToken cancellationToken = default);

    //Task<IReadOnlyList<Food>> SearchAsync(
    //    string query,
    //    CancellationToken cancellationToken = default);




    Task<(List<Food> Items, int TotalCount)> SearchAsync(
     string? searchTerm,
     int? categoryId,
     string sortBy,
     bool sortDescending,
     int pageNumber,
     int pageSize,
     CancellationToken cancellationToken = default);
}