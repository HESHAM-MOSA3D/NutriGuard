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
        if (profile.ActivityLevel is null ||
    profile.DietType is null ||
    profile.Goal is null)
        {
            return NutritionTargetResponseDto.Failure(
                "Health profile is incomplete.");
        }



        var bmr = CalculateBmr(
            profile.Gender,
            profile.Weight,
            profile.Height,
            age);

        var tdee = bmr * GetActivityFactor(profile.ActivityLevel.Value);

        var dailyCalories = CalculateDailyCalories(
            tdee,
            profile.Goal.Value);

        var proteinCalories =
            dailyCalories * GetProteinRatio(profile.DietType.Value);

        var carbsCalories =
            dailyCalories * GetCarbsRatio(profile.DietType.Value);

        var fatCalories =
            dailyCalories * GetFatRatio(profile.DietType.Value);
        var dto = new NutritionTargetDto
        {
            Bmr = Math.Round(bmr, 2),

            Tdee = Math.Round(tdee, 2),

            DailyCalories = Math.Round(dailyCalories, 2),

            ProteinGrams = Math.Round(proteinCalories / 4, 2),

            CarbsGrams = Math.Round(carbsCalories / 4, 2),

            FatGrams = Math.Round(fatCalories / 9, 2)
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

    private static double GetActivityFactor(ActivityLevel activityLevel)
    {
        return activityLevel switch
        {
            ActivityLevel.Sedentary => 1.20,
            ActivityLevel.LightlyActive => 1.375,
            ActivityLevel.ModeratelyActive => 1.55,
            ActivityLevel.VeryActive => 1.725,
            ActivityLevel.ExtremelyActive => 1.90,
            _ => 1.20
        };
    }



    private static double CalculateDailyCalories(
    double tdee,
    Goal goal)
    {
        return goal switch
        {
            Goal.LoseWeight => tdee - 500,
            Goal.MaintainWeight => tdee,
            Goal.GainWeight => tdee + 300,
            _ => tdee
        };
    }



    private static double GetProteinRatio(DietType dietType)
    {
        return dietType switch
        {
            DietType.Balanced => 0.30,
            DietType.LowCarb => 0.35,
            DietType.Vegan => 0.20,
            _ => 0.30
        };
    }



    private static double GetCarbsRatio(DietType dietType)
    {
        return dietType switch
        {
            DietType.Balanced => 0.40,
            DietType.LowCarb => 0.25,
            DietType.Vegan => 0.55,
            _ => 0.40
        };
    }



    private static double GetFatRatio(DietType dietType)
    {
        return dietType switch
        {
            DietType.Balanced => 0.30,
            DietType.LowCarb => 0.40,
            DietType.Vegan => 0.25,
            _ => 0.30
        };
    }




}