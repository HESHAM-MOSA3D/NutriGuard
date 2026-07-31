using NutriGuard.Application.DTOs.Tracking;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Application.Interfaces.Services;

namespace NutriGuard.Application.Services;

public class NutritionRuleEngine : INutritionRuleEngine
{
    private readonly IFoodPreferenceRepository _foodPreferenceRepository;
    private readonly IFoodTagAssignmentRepository _foodTagAssignmentRepository;

    public NutritionRuleEngine(
        IFoodPreferenceRepository foodPreferenceRepository,
        IFoodTagAssignmentRepository foodTagAssignmentRepository)
    {
        _foodPreferenceRepository = foodPreferenceRepository;
        _foodTagAssignmentRepository = foodTagAssignmentRepository;
    }

    public async Task<MealValidationResult> ValidateMealAsync(
        int healthProfileId,
        IEnumerable<int> mealFoodIds,
        CancellationToken cancellationToken = default)
    {
        var result = new MealValidationResult();

        await ValidateAllergies(
            healthProfileId,
            mealFoodIds,
            result,
            cancellationToken);

        await ValidateDiet(
            healthProfileId,
            mealFoodIds,
            result,
            cancellationToken);

        await ValidateGoal(
            healthProfileId,
            mealFoodIds,
            result,
            cancellationToken);

        await ValidateTraditionalFoods(
            healthProfileId,
            mealFoodIds,
            result,
            cancellationToken);

        return result;
    }

    private Task ValidateAllergies(
        int healthProfileId,
        IEnumerable<int> mealFoodIds,
        MealValidationResult result,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private Task ValidateDiet(
        int healthProfileId,
        IEnumerable<int> mealFoodIds,
        MealValidationResult result,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private Task ValidateGoal(
        int healthProfileId,
        IEnumerable<int> mealFoodIds,
        MealValidationResult result,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private Task ValidateTraditionalFoods(
        int healthProfileId,
        IEnumerable<int> mealFoodIds,
        MealValidationResult result,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}