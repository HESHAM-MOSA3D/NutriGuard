using NutriGuard.Application.DTOs.FoodPreference;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Domain.Entities;

namespace NutriGuard.Application.Services;

public sealed class FoodPreferenceService : IFoodPreferenceService
{
    private readonly IFoodPreferenceRepository _foodPreferenceRepository;
    private readonly IFoodRepository _foodRepository;

    public FoodPreferenceService(
        IFoodPreferenceRepository foodPreferenceRepository,
        IFoodRepository foodRepository)
    {
        _foodPreferenceRepository = foodPreferenceRepository;
        _foodRepository = foodRepository;
    }

    public async Task<FoodPreferenceResponseDto> AddAsync(
        int healthProfileId,
        AddFoodPreferenceRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var exists = await _foodPreferenceRepository.ExistsAsync(
            healthProfileId,
            request.FoodId,
            cancellationToken);

        if (exists)
        {
            return FoodPreferenceResponseDto.Failure(
                "Food preference already exists.");
        }

        var food = await _foodRepository.GetByIdAsync(
            request.FoodId,
            cancellationToken);

        if (food is null)
        {
            return FoodPreferenceResponseDto.Failure(
                "Food not found.");
        }

        var preference = new FoodPreference
        {
            HealthProfileId = healthProfileId,
            FoodId = request.FoodId,
            PreferenceType = request.PreferenceType,
            CreatedAt = DateTime.UtcNow
        };

        await _foodPreferenceRepository.AddAsync(
            preference,
            cancellationToken);

        await _foodPreferenceRepository.SaveChangesAsync(
            cancellationToken);

        return FoodPreferenceResponseDto.Success(
            new FoodPreferenceDto
            {
                Id = preference.Id,
                FoodId = preference.FoodId,
                FoodName = food.Name,
                PreferenceType = preference.PreferenceType,
                CreatedAt = preference.CreatedAt
            },
            "Food preference added successfully.");
    }

    public async Task<FoodPreferenceListResponseDto> GetByHealthProfileIdAsync(
        int healthProfileId,
        CancellationToken cancellationToken = default)
    {
        var preferences = await _foodPreferenceRepository
            .GetByHealthProfileIdAsync(
                healthProfileId,
                cancellationToken);

        var data = preferences.Select(x => new FoodPreferenceDto
        {
            Id = x.Id,
            FoodId = x.FoodId,
            FoodName = x.Food.Name,
            PreferenceType = x.PreferenceType,
            CreatedAt = x.CreatedAt
        });

        return FoodPreferenceListResponseDto.Success(data);
    }

    public async Task<FoodPreferenceResponseDto> RemoveAsync(
        int healthProfileId,
        int foodId,
        CancellationToken cancellationToken = default)
    {
        var preference = await _foodPreferenceRepository.GetAsync(
            healthProfileId,
            foodId,
            cancellationToken);

        if (preference is null)
        {
            return FoodPreferenceResponseDto.Failure(
                "Food preference not found.");
        }

        _foodPreferenceRepository.Delete(preference);

        await _foodPreferenceRepository.SaveChangesAsync(
            cancellationToken);

        return FoodPreferenceResponseDto.Success(
            new FoodPreferenceDto
            {
                Id = preference.Id,
                FoodId = preference.FoodId,
                FoodName = preference.Food.Name,
                PreferenceType = preference.PreferenceType,
                CreatedAt = preference.CreatedAt
            },
            "Food preference removed successfully.");
    }
}