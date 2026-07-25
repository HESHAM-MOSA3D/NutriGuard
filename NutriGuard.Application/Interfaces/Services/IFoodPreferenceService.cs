using NutriGuard.Application.DTOs.FoodPreference;

namespace NutriGuard.Application.Interfaces.Services;

public interface IFoodPreferenceService
{
    Task<FoodPreferenceResponseDto> AddAsync(
        int healthProfileId,
        AddFoodPreferenceRequestDto request,
        CancellationToken cancellationToken = default);

    Task<FoodPreferenceListResponseDto> GetByHealthProfileIdAsync(
        int healthProfileId,
        CancellationToken cancellationToken = default);

    Task<FoodPreferenceResponseDto> RemoveAsync(
        int healthProfileId,
        int foodId,
        CancellationToken cancellationToken = default);
}