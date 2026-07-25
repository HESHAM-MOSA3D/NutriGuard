using NutriGuard.Domain.Entities;

namespace NutriGuard.Application.Interfaces.Repositories;

public interface IFoodPreferenceRepository
    : IGenericRepository<FoodPreference>
{
    Task<IEnumerable<FoodPreference>> GetByHealthProfileIdAsync(
        int healthProfileId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        int healthProfileId,
        int foodId,
        CancellationToken cancellationToken = default);


    Task<FoodPreference?> GetAsync(
    int healthProfileId,
    int foodId,
    CancellationToken cancellationToken = default);
}