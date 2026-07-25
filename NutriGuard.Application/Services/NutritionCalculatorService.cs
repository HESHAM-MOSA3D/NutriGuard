using NutriGuard.Application.DTOs.Nutrition;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Domain.Enums;

namespace NutriGuard.Application.Services;

public sealed class NutritionCalculatorService
    : INutritionCalculatorService
{
    private readonly IHealthProfileRepository _healthProfileRepository;

    public NutritionCalculatorService(
        IHealthProfileRepository healthProfileRepository)
    {
        _healthProfileRepository = healthProfileRepository;
    }

    public async Task<NutritionTargetResponseDto> CalculateAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _healthProfileRepository
            .GetByUserIdAsync(userId, cancellationToken);

        if (profile is null)
        {
            return NutritionTargetResponseDto.Failure(
                "Health profile not found.");
        }

        var age = CalculateAge(profile.DateOfBirth);

        var bmr = CalculateBmr(
            profile.Gender,
            profile.Weight,
            profile.Height,
            age);

        var dto = new NutritionTargetDto
        {
            Bmr = Math.Round(bmr, 2),

            Tdee = 0,

            DailyCalories = 0,

            ProteinGrams = 0,

            CarbsGrams = 0,

            FatGrams = 0
        };

        return NutritionTargetResponseDto.Success(dto);
    }

    private static int CalculateAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var age = today.Year - dateOfBirth.Year;

        if (today < dateOfBirth.AddYears(age))
        {
            age--;
        }

        return age;
    }

    private static double CalculateBmr(
        Gender gender,
        double weight,
        double height,
        int age)
    {
        if (gender == Gender.Male)
        {
            return (10 * weight)
                 + (6.25 * height)
                 - (5 * age)
                 + 5;
        }

        return (10 * weight)
             + (6.25 * height)
             - (5 * age)
             - 161;
    }
}