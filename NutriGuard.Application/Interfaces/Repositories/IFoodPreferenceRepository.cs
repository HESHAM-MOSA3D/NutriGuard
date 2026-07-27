using NutriGuard.Domain.Entities;

public interface IFoodPreferenceRepository
{
    Task AddAsync(
        FoodPreference preference,
        CancellationToken cancellationToken = default);

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

    Task UpdateAsync(
        FoodPreference preference,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        FoodPreference preference,
        CancellationToken cancellationToken = default);
}