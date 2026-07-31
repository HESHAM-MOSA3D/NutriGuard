using NutriGuard.Domain.Entities;

namespace NutriGuard.Application.Interfaces.Repositories;

public interface IFoodTagAssignmentRepository
{
    Task<List<FoodTagAssignment>> GetByFoodIdsAsync(
        IEnumerable<int> foodIds,
        CancellationToken cancellationToken = default);

    Task<Dictionary<int, List<int>>> GetFoodTagsMapAsync(
        IEnumerable<int> foodIds,
        CancellationToken cancellationToken = default);
}