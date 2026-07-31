using NutriGuard.Application.DTOs.Nutrition;
using NutriGuard.Application.DTOs.Tracking;

namespace NutriGuard.Application.Interfaces.Services;

public interface INutritionRuleEngine
{
    Task<MealValidationResult> ValidateMealAsync(
        int healthProfileId,
        IEnumerable<int> mealFoodIds,
        CancellationToken cancellationToken = default);

    Task<MealValidationResult> ValidateMealItemsAsync(
        string userId,
        IEnumerable<CreateMealItemDto> mealItems,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<FoodEligibilityDto> CheckFoodEligibilityAsync(
        string userId,
        int foodId,
        CancellationToken cancellationToken = default);

    Task<List<FoodEligibilityDto>> FilterEligibleFoodsAsync(
        string userId,
        IEnumerable<int> foodIds,
        CancellationToken cancellationToken = default);

    Task<List<RecipeEligibilityDto>> FilterEligibleRecipesAsync(
        string userId,
        IEnumerable<int> recipeIds,
        CancellationToken cancellationToken = default);
}