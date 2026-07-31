using Moq;
using NutriGuard.Application.DTOs.Nutrition;
using NutriGuard.Application.DTOs.Tracking;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Application.Services;
using NutriGuard.Domain.Entities;
using NutriGuard.Domain.Enums;
using Xunit;

namespace NutriGuard.Tests;

public class NutritionRuleEngineTests
{
    private readonly Mock<IHealthProfileRepository> _healthProfileRepoMock = new();
    private readonly Mock<IFoodPreferenceRepository> _foodPrefRepoMock = new();
    private readonly Mock<IFoodTagAssignmentRepository> _foodTagRepoMock = new();
    private readonly Mock<IFoodRepository> _foodRepoMock = new();
    private readonly Mock<IRecipeRepository> _recipeRepoMock = new();
    private readonly Mock<IFoodUnitConversionRepository> _unitConversionRepoMock = new();
    private readonly Mock<INutritionCalculatorService> _calculatorMock = new();
    private readonly Mock<IMealLogRepository> _mealLogRepoMock = new();

    private readonly NutritionRuleEngine _engine;

    public NutritionRuleEngineTests()
    {
        _engine = new NutritionRuleEngine(
            _healthProfileRepoMock.Object,
            _foodPrefRepoMock.Object,
            _foodTagRepoMock.Object,
            _foodRepoMock.Object,
            _recipeRepoMock.Object,
            _unitConversionRepoMock.Object,
            _calculatorMock.Object,
            _mealLogRepoMock.Object);
    }

    [Fact]
    public async Task ValidateMealItemsAsync_ShouldReturnError_WhenAllergyTagDetected()
    {
        // Arrange
        string userId = "user-1";
        var profile = new HealthProfile { Id = 1, UserId = userId, DietType = DietType.Balanced, Goal = Goal.MaintainWeight };
        _healthProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(profile);

        var milkFood = new Food { Id = 10, Name = "Full Cream Milk", Energy = 60, Protein = 3, Carbohydrate = 5, Fat = 3 };
        _foodRepoMock.Setup(r => r.GetFoodsByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Food> { milkFood });

        _foodPrefRepoMock.Setup(r => r.GetAllergiesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FoodPreference>());

        // Food 10 has Milk Allergy tag (1)
        _foodTagRepoMock.Setup(r => r.GetFoodTagsMapAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, List<int>> { { 10, new List<int> { (int)FoodTagType.Allergy_Milk } } });

        _calculatorMock.Setup(c => c.CalculateAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(NutritionTargetResponseDto.Success(new NutritionTargetDto { DailyCalories = 2000, ProteinGrams = 100, CarbsGrams = 200, FatGrams = 60 }));

        _mealLogRepoMock.Setup(m => m.GetUserMealLogsByDateAsync(userId, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MealLog>());

        var mealItems = new List<CreateMealItemDto> { new() { FoodId = 10, Quantity = 200, Unit = Unit.Milliliter } };

        // Act
        var result = await _engine.ValidateMealItemsAsync(userId, mealItems, DateOnly.FromDateTime(DateTime.UtcNow));

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Allergy Warning") && e.Contains("Milk"));
    }

    [Fact]
    public async Task ValidateMealItemsAsync_ShouldReturnError_WhenVeganUserEatsMeatOrMilk()
    {
        // Arrange
        string userId = "vegan-user";
        var profile = new HealthProfile { Id = 2, UserId = userId, DietType = DietType.Vegan, Goal = Goal.MaintainWeight };
        _healthProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(profile);

        var chickenFood = new Food { Id = 20, Name = "Grilled Chicken", FoodCategoryId = 1, Energy = 165, Protein = 31, Carbohydrate = 0, Fat = 3.6m };
        _foodRepoMock.Setup(r => r.GetFoodsByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Food> { chickenFood });

        _foodPrefRepoMock.Setup(r => r.GetAllergiesAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FoodPreference>());

        _foodTagRepoMock.Setup(r => r.GetFoodTagsMapAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, List<int>>());

        _calculatorMock.Setup(c => c.CalculateAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(NutritionTargetResponseDto.Success(new NutritionTargetDto { DailyCalories = 2000, ProteinGrams = 100, CarbsGrams = 200, FatGrams = 60 }));

        _mealLogRepoMock.Setup(m => m.GetUserMealLogsByDateAsync(userId, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MealLog>());

        var mealItems = new List<CreateMealItemDto> { new() { FoodId = 20, Quantity = 150, Unit = Unit.Gram } };

        // Act
        var result = await _engine.ValidateMealItemsAsync(userId, mealItems, DateOnly.FromDateTime(DateTime.UtcNow));

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Diet Violation") && e.Contains("Vegan"));
    }

    [Fact]
    public async Task ValidateMealItemsAsync_ShouldProvideTip_ForTraditionalEgyptianKoshary()
    {
        // Arrange
        string userId = "egyptian-user";
        var profile = new HealthProfile { Id = 3, UserId = userId, DietType = DietType.Balanced, Goal = Goal.LoseWeight };
        _healthProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(profile);

        var koshary = new Food { Id = 30, Name = "Koshary Egyptian Dish", Energy = 350, Protein = 12, Carbohydrate = 65, Fat = 6 };
        _foodRepoMock.Setup(r => r.GetFoodsByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Food> { koshary });

        _foodPrefRepoMock.Setup(r => r.GetAllergiesAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FoodPreference>());

        _foodTagRepoMock.Setup(r => r.GetFoodTagsMapAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, List<int>> { { 30, new List<int> { (int)FoodTagType.TraditionalEgyptian } } });

        _calculatorMock.Setup(c => c.CalculateAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(NutritionTargetResponseDto.Success(new NutritionTargetDto { DailyCalories = 1800, ProteinGrams = 120, CarbsGrams = 180, FatGrams = 50 }));

        _mealLogRepoMock.Setup(m => m.GetUserMealLogsByDateAsync(userId, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MealLog>());

        var mealItems = new List<CreateMealItemDto> { new() { FoodId = 30, Quantity = 250, Unit = Unit.Gram } };

        // Act
        var result = await _engine.ValidateMealItemsAsync(userId, mealItems, DateOnly.FromDateTime(DateTime.UtcNow));

        // Assert
        Assert.True(result.IsValid);
        Assert.Contains(result.Tips, t => t.Contains("Egyptian Cuisine Tip") && t.Contains("Koshary"));
    }

    [Fact]
    public async Task ValidateMealItemsAsync_ShouldWarn_WhenCaloriesExceedRemainingBudget()
    {
        // Arrange
        string userId = "calorie-user";
        var profile = new HealthProfile { Id = 4, UserId = userId, DietType = DietType.Balanced, Goal = Goal.LoseWeight };
        _healthProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(profile);

        var heavyFood = new Food { Id = 40, Name = "Heavy Fiteer", Energy = 900, Protein = 15, Carbohydrate = 110, Fat = 45 };
        _foodRepoMock.Setup(r => r.GetFoodsByIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Food> { heavyFood });

        _foodPrefRepoMock.Setup(r => r.GetAllergiesAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FoodPreference>());

        _foodTagRepoMock.Setup(r => r.GetFoodTagsMapAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, List<int>>());

        // Target: 1500 kcal
        _calculatorMock.Setup(c => c.CalculateAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(NutritionTargetResponseDto.Success(new NutritionTargetDto { DailyCalories = 1500, ProteinGrams = 100, CarbsGrams = 150, FatGrams = 40 }));

        // Existing consumed: 1000 kcal -> Remaining: 500 kcal
        var existingLog = new MealLog
        {
            MealItems = new List<MealItem>
            {
                new() { Quantity = 200, Food = new Food { Energy = 500 } }
            }
        };
        _mealLogRepoMock.Setup(m => m.GetUserMealLogsByDateAsync(userId, It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MealLog> { existingLog });

        var mealItems = new List<CreateMealItemDto> { new() { FoodId = 40, Quantity = 100, Unit = Unit.Gram } };

        // Act
        var result = await _engine.ValidateMealItemsAsync(userId, mealItems, DateOnly.FromDateTime(DateTime.UtcNow));

        // Assert
        Assert.True(result.IsValid); // No strict allergy/vegan error
        Assert.NotNull(result.CalorieBreakdown);
        Assert.True(result.CalorieBreakdown.ExceedsRemainingBudget);
        Assert.Contains(result.Warnings, w => w.Contains("Calorie Overflow Warning"));
    }

    [Fact]
    public async Task CheckFoodEligibilityAsync_ShouldReturnIneligible_WhenFoodHasAllergen()
    {
        // Arrange
        string userId = "user-allergy";
        var profile = new HealthProfile { Id = 5, UserId = userId, DietType = DietType.Balanced, Goal = Goal.MaintainWeight };
        _healthProfileRepoMock.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(profile);

        var nutFood = new Food { Id = 50, Name = "Peanut Butter", Energy = 588, Protein = 25, Carbohydrate = 20, Fat = 50 };
        _foodRepoMock.Setup(r => r.GetByIdAsync(50, It.IsAny<CancellationToken>())).ReturnsAsync(nutFood);

        _foodPrefRepoMock.Setup(r => r.GetAllergiesAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FoodPreference>());

        _foodTagRepoMock.Setup(r => r.GetFoodTagsMapAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<int, List<int>> { { 50, new List<int> { (int)FoodTagType.Allergy_Nuts } } });

        // Act
        var eligibility = await _engine.CheckFoodEligibilityAsync(userId, 50);

        // Assert
        Assert.False(eligibility.IsEligible);
        Assert.Contains(eligibility.IneligibilityReasons, r => r.Contains("Allergy Warning") && r.Contains("Nuts"));
    }
}
