namespace NutriGuard.Application.DTOs.Nutrition;

public sealed class NutritionTargetDto
{
    public double Bmr { get; init; }

    public double Tdee { get; init; }

    public double DailyCalories { get; init; }

    public double ProteinGrams { get; init; }

    public double CarbsGrams { get; init; }

    public double FatGrams { get; init; }
}