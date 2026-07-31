using NutriGuard.Application.DTOs.Tracking;

namespace NutriGuard.Application.Interfaces.Services;

public interface INutritionRuleEngine
{
    Task<MealValidationResult> ValidateMealAsync(
        int healthProfileId,
        IEnumerable<int> mealFoodIds,
        CancellationToken cancellationToken = default);
}