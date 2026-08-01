using NutriGuard.Application.DTOs.Nutrition;
using NutriGuard.Application.DTOs.Tracking;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Domain.Entities;
using NutriGuard.Domain.Enums;

namespace NutriGuard.Application.Services;

public class NutritionRuleEngine : INutritionRuleEngine
{
    private readonly IHealthProfileRepository _healthProfileRepository;
    private readonly IFoodPreferenceRepository _foodPreferenceRepository;
    private readonly IFoodTagAssignmentRepository _foodTagAssignmentRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IFoodUnitConversionRepository _foodUnitConversionRepository;
    private readonly INutritionCalculatorService _nutritionCalculatorService;
    private readonly IMealLogRepository _mealLogRepository;

    public NutritionRuleEngine(
        IHealthProfileRepository healthProfileRepository,
        IFoodPreferenceRepository foodPreferenceRepository,
        IFoodTagAssignmentRepository foodTagAssignmentRepository,
        IFoodRepository foodRepository,
        IRecipeRepository recipeRepository,
        IFoodUnitConversionRepository foodUnitConversionRepository,
        INutritionCalculatorService nutritionCalculatorService,
        IMealLogRepository mealLogRepository)
    {
        _healthProfileRepository = healthProfileRepository;
        _foodPreferenceRepository = foodPreferenceRepository;
        _foodTagAssignmentRepository = foodTagAssignmentRepository;
        _foodRepository = foodRepository;
        _recipeRepository = recipeRepository;
        _foodUnitConversionRepository = foodUnitConversionRepository;
        _nutritionCalculatorService = nutritionCalculatorService;
        _mealLogRepository = mealLogRepository;
    }

    public async Task<MealValidationResult> ValidateMealAsync(
        int healthProfileId,
        IEnumerable<int> mealFoodIds,
        CancellationToken cancellationToken = default)
    {
        var profile = await _healthProfileRepository.GetByIdAsync(healthProfileId, cancellationToken);
        if (profile == null)
        {
            var errResult = new MealValidationResult();
            errResult.AddError("Health profile not found.");
            return errResult;
        }

        var items = mealFoodIds.Select(id => new CreateMealItemDto
        {
            FoodId = id,
            Quantity = 100,
            Unit = Unit.Gram
        });

        return await ValidateMealItemsAsync(profile.UserId, items, DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
    }

    public async Task<MealValidationResult> ValidateMealItemsAsync(
        string userId,
        IEnumerable<CreateMealItemDto> mealItems,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var result = new MealValidationResult();

        var profile = await _healthProfileRepository.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null)
        {
            result.AddError("Health profile not found.");
            return result;
        }

        var itemsList = mealItems.ToList();
        if (!itemsList.Any())
        {
            return result;
        }

        var foodIds = itemsList.Select(x => x.FoodId).Distinct().ToList();
        var foodsList = await _foodRepository.GetFoodsByIdsAsync(foodIds, cancellationToken);
        var foodDict = foodsList.ToDictionary(f => f.Id);

        var foodTagMap = await _foodTagAssignmentRepository.GetFoodTagsMapAsync(foodIds, cancellationToken);

        // Batch load conversions for all food items
        var conversions = await _foodUnitConversionRepository.GetByFoodIdsAsync(foodIds, cancellationToken);
        var conversionLookup = conversions.ToDictionary(x => (x.FoodId, x.Unit), x => (double)x.GramsPerUnit);

        // Compute total meal nutritional values
        double totalCalories = 0;
        double totalProtein = 0;
        double totalCarbs = 0;
        double totalFat = 0;

        foreach (var item in itemsList)
        {
            if (!foodDict.TryGetValue(item.FoodId, out var food))
            {
                result.AddError($"Food with ID {item.FoodId} not found.");
                continue;
            }

            double grams = ConvertToGrams(item.FoodId, item.Quantity, item.Unit, conversionLookup);
            double multiplier = grams / 100.0;

            totalCalories += (double)(food.Energy ?? 0) * multiplier;
            totalProtein += (double)(food.Protein ?? 0) * multiplier;
            totalCarbs += (double)(food.Carbohydrate ?? 0) * multiplier;
            totalFat += (double)(food.Fat ?? 0) * multiplier;
        }

        // 1. Validate Allergies
        await ValidateAllergies(profile.Id, foodDict, foodTagMap, result, cancellationToken);

        // 2. Validate Diet
        ValidateDiet(profile, foodDict, foodTagMap, totalCalories, totalCarbs, totalProtein, result);

        // 3. Validate Goal
        ValidateGoal(profile, foodTagMap, totalCalories, totalProtein, result);

        // 4. Validate Calories
        await ValidateCalories(userId, date, totalCalories, result, cancellationToken);

        // 5. Validate Macronutrients
        await ValidateMacronutrients(userId, date, totalProtein, totalCarbs, totalFat, result, cancellationToken);

        // 6. Validate Traditional Egyptian Food Rules
        ValidateTraditionalFoods(foodDict, foodTagMap, result, profile.DietType);

        return result;
    }

    public async Task<FoodEligibilityDto> CheckFoodEligibilityAsync(
        string userId,
        int foodId,
        CancellationToken cancellationToken = default)
    {
        var profile = await _healthProfileRepository.GetByUserIdAsync(userId, cancellationToken);
        var food = await _foodRepository.GetByIdAsync(foodId, cancellationToken);

        if (food == null)
        {
            return new FoodEligibilityDto
            {
                FoodId = foodId,
                FoodName = "Unknown",
                IsEligible = false,
                IneligibilityReasons = new List<string> { "Food not found." }
            };
        }

        var tagMap = await _foodTagAssignmentRepository.GetFoodTagsMapAsync(new[] { foodId }, cancellationToken);
        var foodDict = new Dictionary<int, Food> { { foodId, food } };

        var validationResult = new MealValidationResult();

        if (profile != null)
        {
            await ValidateAllergies(profile.Id, foodDict, tagMap, validationResult, cancellationToken);
            ValidateDiet(profile, foodDict, tagMap, (double)(food.Energy ?? 0), (double)(food.Carbohydrate ?? 0), (double)(food.Protein ?? 0), validationResult);
        }

        return BuildFoodEligibilityDto(foodId, food, tagMap, validationResult);
    }

    public async Task<List<FoodEligibilityDto>> FilterEligibleFoodsAsync(
        string userId,
        IEnumerable<int> foodIds,
        CancellationToken cancellationToken = default)
    {
        var distinctFoodIds = foodIds.Distinct().ToList();
        var profile = await _healthProfileRepository.GetByUserIdAsync(userId, cancellationToken);
        var foods = await _foodRepository.GetFoodsByIdsAsync(distinctFoodIds, cancellationToken);
        var foodDict = foods.ToDictionary(f => f.Id);
        var tagMap = await _foodTagAssignmentRepository.GetFoodTagsMapAsync(distinctFoodIds, cancellationToken);

        var resultList = new List<FoodEligibilityDto>();

        foreach (var foodId in distinctFoodIds)
        {
            if (!foodDict.TryGetValue(foodId, out var food))
            {
                resultList.Add(new FoodEligibilityDto
                {
                    FoodId = foodId,
                    FoodName = "Unknown",
                    IsEligible = false,
                    IneligibilityReasons = new List<string> { "Food not found." }
                });
                continue;
            }

            var singleFoodDict = new Dictionary<int, Food> { { foodId, food } };
            var singleTagMap = new Dictionary<int, List<int>> { { foodId, tagMap.TryGetValue(foodId, out var tags) ? tags : new List<int>() } };
            var validationResult = new MealValidationResult();

            if (profile != null)
            {
                await ValidateAllergies(profile.Id, singleFoodDict, singleTagMap, validationResult, cancellationToken);
                ValidateDiet(profile, singleFoodDict, singleTagMap, (double)(food.Energy ?? 0), (double)(food.Carbohydrate ?? 0), (double)(food.Protein ?? 0), validationResult);
            }

            resultList.Add(BuildFoodEligibilityDto(foodId, food, singleTagMap, validationResult));
        }

        return resultList;
    }

    public async Task<List<RecipeEligibilityDto>> FilterEligibleRecipesAsync(
        string userId,
        IEnumerable<int> recipeIds,
        CancellationToken cancellationToken = default)
    {
        var profile = await _healthProfileRepository.GetByUserIdAsync(userId, cancellationToken);
        var resultList = new List<RecipeEligibilityDto>();

        foreach (var recipeId in recipeIds.Distinct())
        {
            var recipe = await _recipeRepository.GetDetailsByIdAsync(recipeId, cancellationToken);
            if (recipe == null)
            {
                resultList.Add(new RecipeEligibilityDto
                {
                    RecipeId = recipeId,
                    RecipeTitle = "Unknown",
                    IsEligible = false,
                    IneligibilityReasons = new List<string> { "Recipe not found." }
                });
                continue;
            }

            var ingredientFoodIds = recipe.RecipeIngredients.Select(ri => ri.FoodId).Distinct().ToList();
            var foodDict = (await _foodRepository.GetFoodsByIdsAsync(ingredientFoodIds, cancellationToken)).ToDictionary(f => f.Id);
            var tagMap = await _foodTagAssignmentRepository.GetFoodTagsMapAsync(ingredientFoodIds, cancellationToken);

            var validationResult = new MealValidationResult();
            if (profile != null)
            {
                await ValidateAllergies(profile.Id, foodDict, tagMap, validationResult, cancellationToken);
                ValidateDiet(profile, foodDict, tagMap, 0, 0, 0, validationResult);
            }

            bool isTraditional = IsTraditionalEgyptianRecipeName(recipe.Name);

            resultList.Add(new RecipeEligibilityDto
            {
                RecipeId = recipeId,
                RecipeTitle = recipe.Name,
                IsEligible = validationResult.IsValid,
                IneligibilityReasons = validationResult.Errors,
                Warnings = validationResult.Warnings,
                IsTraditionalEgyptian = isTraditional
            });
        }

        return resultList;
    }

    #region Helper Methods

    private static FoodEligibilityDto BuildFoodEligibilityDto(
        int foodId,
        Food food,
        Dictionary<int, List<int>> tagMap,
        MealValidationResult validationResult)
    {
        var tags = tagMap.TryGetValue(foodId, out var tagIds) ? tagIds : new List<int>();
        var healthTagNames = tags.Select(t => ((FoodTagType)t).ToString()).ToList();
        bool isTraditional = tags.Contains((int)FoodTagType.TraditionalEgyptian) || IsTraditionalEgyptianName(food.Name);

        return new FoodEligibilityDto
        {
            FoodId = foodId,
            FoodName = food.Name,
            IsEligible = validationResult.IsValid,
            IneligibilityReasons = validationResult.Errors,
            Warnings = validationResult.Warnings,
            HealthTags = healthTagNames,
            IsTraditionalEgyptian = isTraditional
        };
    }

    private static bool IsTraditionalEgyptianRecipeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        return name.Contains("كشري", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("Koshary", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("فطير", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("Fiteer", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("محشي", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("Mahshi", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("ملوخية", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("Molokhia", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("فول", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("Ful", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("طعمية", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("Taameya", StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Rule Implementations

    private async Task ValidateAllergies(
        int healthProfileId,
        Dictionary<int, Food> foodDict,
        Dictionary<int, List<int>> foodTagMap,
        MealValidationResult result,
        CancellationToken cancellationToken)
    {
        var userAllergyPrefs = await _foodPreferenceRepository.GetAllergiesAsync(healthProfileId, cancellationToken);
        var allergyFoodIds = userAllergyPrefs.Select(p => p.FoodId).ToHashSet();

        // Get the allergen tags from the user's allergy foods to determine which allergen types they're allergic to
        var allergyFoodTagIds = await _foodTagAssignmentRepository.GetFoodTagsMapAsync(allergyFoodIds, cancellationToken);
        var userAllergenTagTypes = new HashSet<FoodTagType>();
        foreach (var tagList in allergyFoodTagIds.Values)
        {
            foreach (var tagId in tagList)
            {
                var tagEnum = (FoodTagType)tagId;
                if (IsAllergyTag(tagEnum))
                {
                    userAllergenTagTypes.Add(tagEnum);
                }
            }
        }

        foreach (var food in foodDict.Values)
        {
            // Check explicit food allergy
            if (allergyFoodIds.Contains(food.Id))
            {
                result.AddError($"Allergy Conflict: '{food.Name}' is listed in your explicit allergy preferences.");
            }

            // Check allergen tags against user's actual allergens
            if (foodTagMap.TryGetValue(food.Id, out var tags))
            {
                foreach (var tagId in tags)
                {
                    var tagEnum = (FoodTagType)tagId;
                    if (IsAllergyTag(tagEnum))
                    {
                        string allergenName = GetFriendlyAllergenName(tagEnum);

                        // Only generate ERROR if user is actually allergic to this allergen type
                        if (userAllergenTagTypes.Contains(tagEnum))
                        {
                            result.AddError($"Allergy Conflict: '{food.Name}' contains {allergenName}, which you are allergic to.");
                        }
                        else
                        {
                            // Only add informational tip, not a warning
                            result.AddTip($"Ingredient Notice: Contains {allergenName}.");
                        }
                    }
                }
            }
        }
    }

    private static bool IsAllergyTag(FoodTagType tag)
    {
        return tag is FoodTagType.Allergy_Milk or FoodTagType.Allergy_Egg or FoodTagType.Allergy_Gluten
                   or FoodTagType.Allergy_Nuts or FoodTagType.Allergy_Fish or FoodTagType.Allergy_Soy
                   or FoodTagType.Allergy_Meat;
    }

    private static string GetFriendlyAllergenName(FoodTagType tag) => tag switch
    {
        FoodTagType.Allergy_Milk => "Milk / Dairy",
        FoodTagType.Allergy_Egg => "Egg",
        FoodTagType.Allergy_Gluten => "Gluten",
        FoodTagType.Allergy_Nuts => "Nuts",
        FoodTagType.Allergy_Fish => "Fish",
        FoodTagType.Allergy_Soy => "Soy",
        FoodTagType.Allergy_Meat => "Meat",
        _ => tag.ToString()
    };

    private static void ValidateDiet(
        HealthProfile profile,
        Dictionary<int, Food> foodDict,
        Dictionary<int, List<int>> foodTagMap,
        double totalCalories,
        double totalCarbs,
        double totalProtein,
        MealValidationResult result)
    {
        if (profile.DietType == null) return;

        switch (profile.DietType.Value)
        {
            case DietType.Vegan:
                foreach (var food in foodDict.Values)
                {
                    foodTagMap.TryGetValue(food.Id, out var tags);
                    bool hasMilk = tags != null && tags.Contains((int)FoodTagType.Allergy_Milk);
                    bool hasEgg = tags != null && tags.Contains((int)FoodTagType.Allergy_Egg);
                    bool hasFish = tags != null && tags.Contains((int)FoodTagType.Allergy_Fish);
                    bool hasMeat = tags != null && tags.Contains((int)FoodTagType.Allergy_Meat);

                    bool isNonVeganCategory = food.FoodCategoryId == 1 || food.FoodCategoryId == 2 || food.FoodCategoryId == 3; // Meat/Poultry/Fish/Egg categories

                    if (hasMilk || hasEgg || hasFish || hasMeat || isNonVeganCategory)
                    {
                        result.AddError($"Diet Violation: '{food.Name}' contains animal products and is not suitable for a Vegan diet.");
                    }
                }
                break;

            case DietType.LowCarb:
                double carbCalories = totalCarbs * 4.0;
                double carbRatio = totalCalories > 0 ? (carbCalories / totalCalories) : 0;
                if (totalCarbs > 40 || carbRatio > 0.35)
                {
                    result.AddWarning($"Diet Warning: Total meal carbohydrates ({Math.Round(totalCarbs, 1)}g) exceed recommended Low-Carb guidelines.");
                }
                break;

            case DietType.Balanced:
                // Standard balanced diet validation - no restrictions
                break;
        }
    }

    private static void ValidateGoal(
        HealthProfile profile,
        Dictionary<int, List<int>> foodTagMap,
        double totalCalories,
        double totalProtein,
        MealValidationResult result)
    {
        if (profile.Goal == null) return;

        switch (profile.Goal.Value)
        {
            case Goal.LoseWeight:
                if (totalCalories > 700)
                {
                    result.AddWarning($"Goal Warning: High calorie meal ({Math.Round(totalCalories)} kcal). For weight loss goals, consider lighter portions.");
                    result.IsRecommended = false;
                }

                bool hasWeightLossFriendly = foodTagMap.Values.Any(tags => tags.Contains((int)FoodTagType.WeightLossFriendly) || tags.Contains((int)FoodTagType.HighFiber));
                if (hasWeightLossFriendly && totalCalories <= 700)
                {
                    result.AddTip("Goal Tip: Great choice! This meal contains high-fiber or weight-loss friendly foods.");
                }
                else if (!hasWeightLossFriendly && totalCalories > 500 && totalCalories <= 700)
                {
                    result.AddTip("Goal Tip: Consider replacing with weight-loss friendly options like vegetables, lean proteins, or high-fiber foods.");
                }
                break;

            case Goal.GainWeight:
                if (totalCalories < 250 && totalCalories > 0)
                {
                    result.AddWarning($"Goal Warning: Low calorie meal ({Math.Round(totalCalories)} kcal). For weight gain goals, aim for higher caloric density.");
                    result.IsRecommended = false;
                }

                bool hasGainFriendly = foodTagMap.Values.Any(tags => tags.Contains((int)FoodTagType.WeightGainFriendly) || tags.Contains((int)FoodTagType.MuscleBuilding));
                if (hasGainFriendly && totalCalories >= 250)
                {
                    result.AddTip("Goal Tip: Excellent! This meal includes high calorie density / muscle building components.");
                }
                else if (!hasGainFriendly && totalCalories >= 250)
                {
                    result.AddTip("Goal Tip: Consider adding nutrient-dense foods like nuts, healthy fats, or protein sources to support weight gain.");
                }
                break;

            case Goal.MaintainWeight:
                if (totalCalories > 800)
                {
                    result.AddWarning($"Goal Warning: High calorie meal ({Math.Round(totalCalories)} kcal). For weight maintenance, balance your intake throughout the day.");
                    result.IsRecommended = false;
                }
                if (totalCalories < 150 && totalCalories > 0)
                {
                    result.AddWarning($"Goal Warning: Low calorie meal ({Math.Round(totalCalories)} kcal). Ensure you're meeting your daily nutritional needs.");
                    result.IsRecommended = false;
                }
                if (totalProtein >= 20 && totalCalories >= 150 && totalCalories <= 800)
                {
                    result.AddTip("Goal Tip: Well-balanced protein intake for weight maintenance.");
                }
                break;
        }
    }

    private async Task ValidateCalories(
        string userId,
        DateOnly date,
        double totalCalories,
        MealValidationResult result,
        CancellationToken cancellationToken)
    {
        var targetResult = await _nutritionCalculatorService.CalculateAsync(userId, cancellationToken);
        if (!targetResult.IsSuccess || targetResult.Data == null) return;

        var dailyTarget = targetResult.Data.DailyCalories;
        var existingLogs = await _mealLogRepository.GetUserMealLogsByDateAsync(userId, date, cancellationToken);

        // Batch load conversions for existing meal items
        var existingFoodIds = existingLogs.SelectMany(m => m.MealItems.Select(i => i.FoodId)).Distinct().ToList();
        var conversions = await _foodUnitConversionRepository.GetByFoodIdsAsync(existingFoodIds, cancellationToken);
        var conversionLookup = conversions.ToDictionary(x => (x.FoodId, x.Unit), x => (double)x.GramsPerUnit);

        double alreadyConsumed = CalculateConsumedNutrient(existingLogs, conversionLookup, f => (double)(f.Energy ?? 0));

        double remaining = Math.Max(0, dailyTarget - alreadyConsumed);
        bool exceedsRemaining = totalCalories > remaining && remaining > 0;
        bool isHighCalorie = totalCalories > (dailyTarget * 0.5);

        result.CalorieBreakdown = new MealCalorieValidationDto
        {
            MealCalories = Math.Round(totalCalories, 1),
            RemainingDailyCalories = Math.Round(remaining, 1),
            TargetDailyCalories = Math.Round(dailyTarget, 1),
            ExceedsRemainingBudget = exceedsRemaining,
            IsHighCalorieMeal = isHighCalorie
        };

        if (exceedsRemaining)
        {
            result.AddWarning($"Calorie Overflow Warning: This meal ({Math.Round(totalCalories)} kcal) exceeds your remaining daily budget ({Math.Round(remaining)} kcal).");
            result.AddTip("Consider reducing portion size or splitting this meal across multiple eating occasions.");
            result.IsRecommended = false;
        }

        if (isHighCalorie)
        {
            result.AddWarning($"Calorie Warning: This meal contains {Math.Round(totalCalories)} kcal, which is more than 50% of your total daily target.");
            result.AddTip("Balance your remaining meals to stay within your daily calorie goal.");
            result.IsRecommended = false;
        }
    }

    private async Task ValidateMacronutrients(
        string userId,
        DateOnly date,
        double totalProtein,
        double totalCarbs,
        double totalFat,
        MealValidationResult result,
        CancellationToken cancellationToken)
    {
        var targetResult = await _nutritionCalculatorService.CalculateAsync(userId, cancellationToken);
        if (!targetResult.IsSuccess || targetResult.Data == null) return;

        var targetData = targetResult.Data;
        var existingLogs = await _mealLogRepository.GetUserMealLogsByDateAsync(userId, date, cancellationToken);
        var profile = await _healthProfileRepository.GetByUserIdAsync(userId, cancellationToken);

        // Batch load conversions for existing meal items
        var existingFoodIds = existingLogs.SelectMany(m => m.MealItems.Select(i => i.FoodId)).Distinct().ToList();
        var conversions = await _foodUnitConversionRepository.GetByFoodIdsAsync(existingFoodIds, cancellationToken);
        var conversionLookup = conversions.ToDictionary(x => (x.FoodId, x.Unit), x => (double)x.GramsPerUnit);

        double consumedProtein = CalculateConsumedNutrient(existingLogs, conversionLookup, f => (double)(f.Protein ?? 0));
        double consumedCarbs = CalculateConsumedNutrient(existingLogs, conversionLookup, f => (double)(f.Carbohydrate ?? 0));
        double consumedFat = CalculateConsumedNutrient(existingLogs, conversionLookup, f => (double)(f.Fat ?? 0));

        double remainingProtein = Math.Max(0, targetData.ProteinGrams - consumedProtein);
        double remainingCarbs = Math.Max(0, targetData.CarbsGrams - consumedCarbs);
        double remainingFat = Math.Max(0, targetData.FatGrams - consumedFat);

        bool isLowProtein = totalProtein < 15;
        bool isHighCarb = totalCarbs > 80;
        bool isHighFat = totalFat > 40;

        result.MacroBreakdown = new MealMacroValidationDto
        {
            MealProteinGrams = Math.Round(totalProtein, 1),
            MealCarbsGrams = Math.Round(totalCarbs, 1),
            MealFatGrams = Math.Round(totalFat, 1),

            RemainingProteinGrams = Math.Round(remainingProtein, 1),
            RemainingCarbsGrams = Math.Round(remainingCarbs, 1),
            RemainingFatGrams = Math.Round(remainingFat, 1),

            ExceedsProteinBudget = totalProtein > remainingProtein && remainingProtein > 0,
            ExceedsCarbsBudget = totalCarbs > remainingCarbs && remainingCarbs > 0,
            ExceedsFatBudget = totalFat > remainingFat && remainingFat > 0,

            IsLowProtein = isLowProtein,
            IsHighCarb = isHighCarb,
            IsHighFat = isHighFat
        };

        if (result.MacroBreakdown.ExceedsCarbsBudget)
        {
            result.AddWarning($"Macro Overflow: This meal exceeds your remaining daily carbohydrate budget ({Math.Round(remainingCarbs, 1)}g).");
            result.AddTip("Consider reducing portion size or replacing with lower-carb options like vegetables or lean proteins.");
            result.IsRecommended = false;
        }

        if (result.MacroBreakdown.ExceedsFatBudget)
        {
            result.AddWarning($"Macro Overflow: This meal exceeds your remaining daily fat budget ({Math.Round(remainingFat, 1)}g).");
            result.AddTip("Consider reducing portion size or choosing leaner alternatives to support your nutritional goals.");
            result.IsRecommended = false;
        }

        if (isLowProtein)
        {
            string proteinRecommendation = GetDietSpecificProteinRecommendation(profile?.DietType);
            result.AddWarning($"Macro Warning: Protein is low for your daily goal ({Math.Round(totalProtein, 1)}g). {proteinRecommendation}");
            result.IsRecommended = false;
        }

        if (isHighCarb && !result.MacroBreakdown.ExceedsCarbsBudget)
        {
            result.AddWarning($"Macro Warning: High carbohydrate intake ({Math.Round(totalCarbs, 1)}g) in a single meal.");
            result.AddTip("Consider reducing portion size or replacing with lower-carb options like vegetables or lean proteins.");
            result.IsRecommended = false;
        }

        if (isHighFat && !result.MacroBreakdown.ExceedsFatBudget)
        {
            result.AddWarning($"Macro Warning: High fat content ({Math.Round(totalFat, 1)}g) in a single meal.");
            result.AddTip("Consider reducing portion size or choosing leaner alternatives to support your nutritional goals.");
            result.IsRecommended = false;
        }
    }

    private static void ValidateTraditionalFoods(
        Dictionary<int, Food> foodDict,
        Dictionary<int, List<int>> foodTagMap,
        MealValidationResult result,
        DietType? dietType = null)
    {
        foreach (var food in foodDict.Values)
        {
            bool isTaggedTraditional = foodTagMap.TryGetValue(food.Id, out var tags) && tags.Contains((int)FoodTagType.TraditionalEgyptian);
            bool isTraditionalByName = IsTraditionalEgyptianName(food.Name);

            if (isTaggedTraditional || isTraditionalByName)
            {
                string nameLower = food.Name.ToLower();

                if (nameLower.Contains("كشري") || nameLower.Contains("koshary"))
                {
                    result.AddTip("Egyptian Cuisine Tip: Koshary is rich & delicious, but carb-heavy! Pair with fresh Baladi salad and moderate your portion.");
                }
                else if (nameLower.Contains("فطير") || nameLower.Contains("fiteer") || nameLower.Contains("حواوشي") || nameLower.Contains("hawawshi"))
                {
                    result.AddTip("Egyptian Cuisine Tip: High in fats & calories. Balance with fresh veggies and stay hydrated.");
                }
                else if (nameLower.Contains("محشي") || nameLower.Contains("mahshi"))
                {
                    string proteinTip = dietType == DietType.Vegan
                        ? "Add plant-based protein like lentils or chickpeas for complete macro balance."
                        : "Add lean protein (chicken or meat) for complete macro balance.";
                    result.AddTip($"Egyptian Cuisine Tip: Mahshi provides great vitamins from vegetable skin! {proteinTip}");
                }
                else if (nameLower.Contains("فول") || nameLower.Contains("ful") || nameLower.Contains("طعمية") || nameLower.Contains("taameya"))
                {
                    result.AddTip("Egyptian Cuisine Tip: Ful & Taameya are excellent traditional plant-protein sources! Watch the added oils or tahini.");
                }
                else if (nameLower.Contains("ملوخية") || nameLower.Contains("molokhia"))
                {
                    result.AddTip("Egyptian Cuisine Tip: Molokhia is nutrient-packed & low calorie! Be mindful of ghee in the Ta'leya.");
                }
                else
                {
                    result.AddTip($"Egyptian Cuisine Tip: '{food.Name}' is a classic traditional Egyptian food! Keep your overall daily targets in mind.");
                }

                // Add diet-specific traditional food recommendations
                if (dietType == DietType.Vegan)
                {
                    if (nameLower.Contains("فول") || nameLower.Contains("ful"))
                    {
                        result.AddTip("Vegan Tip: Ful Medames is perfect for your diet - rich in protein and fiber.");
                    }
                    else if (nameLower.Contains("طعمية") || nameLower.Contains("taameya"))
                    {
                        result.AddTip("Vegan Tip: Taameya (Egyptian falafel) is an excellent protein source for vegans.");
                    }
                    else if (nameLower.Contains("ملوخية") || nameLower.Contains("molokhia"))
                    {
                        result.AddTip("Vegan Tip: Molokhia is naturally vegan and packed with nutrients.");
                    }
                    else if (nameLower.Contains("كشري") || nameLower.Contains("koshary"))
                    {
                        result.AddTip("Vegan Tip: Traditional Koshary is naturally vegan and provides complete protein.");
                    }
                }
            }
        }
    }

    private static bool IsTraditionalEgyptianName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        string lower = name.ToLower();
        return lower.Contains("كشري") || lower.Contains("koshary") ||
               lower.Contains("فطير") || lower.Contains("fiteer") ||
               lower.Contains("محشي") || lower.Contains("mahshi") ||
               lower.Contains("ملوخية") || lower.Contains("molokhia") ||
               lower.Contains("فول") || lower.Contains("ful") ||
               lower.Contains("طعمية") || lower.Contains("taameya") ||
               lower.Contains("حواوشي") || lower.Contains("hawawshi") ||
               lower.Contains("ارز بلبن") || lower.Contains("roz bel laban") ||
               lower.Contains("بصارة") || lower.Contains("bessara");
    }

    private static string GetDietSpecificProteinRecommendation(DietType? dietType)
    {
        return dietType switch
        {
            DietType.Vegan => "Recommended vegan sources: Lentils, Chickpeas, Tofu, Tempeh, Quinoa, Soy Milk, and Beans.",
            DietType.LowCarb => "Recommended low-carb sources: Eggs, Greek Yogurt, Cottage Cheese, Fish, Chicken, and Tofu.",
            DietType.Balanced => "Recommended sources: Chicken, Eggs, Greek Yogurt, Fish, Beans, Lentils, and Cottage Cheese.",
            _ => "Recommended sources: Chicken, Eggs, Greek Yogurt, Fish, Beans, Lentils, and Cottage Cheese."
        };
    }

    private static double ConvertToGrams(
        int foodId,
        decimal quantity,
        Unit unit,
        Dictionary<(int FoodId, Unit Unit), double> conversionLookup)
    {
        if (conversionLookup.TryGetValue((foodId, unit), out var gramsPerUnit))
        {
            return (double)quantity * gramsPerUnit;
        }

        return unit switch
        {
            Unit.Gram => (double)quantity,
            Unit.Milliliter => (double)quantity,
            Unit.Tablespoon => (double)quantity * 15,
            Unit.Teaspoon => (double)quantity * 5,
            Unit.Cup => (double)quantity * 240,
            Unit.Piece => (double)quantity * 100,
            _ => (double)quantity
        };
    }

    private static double CalculateConsumedNutrient(
        IEnumerable<MealLog> mealLogs,
        Dictionary<(int FoodId, Unit Unit), double> conversionLookup,
        Func<Food, double> nutrientSelector)
    {
        return mealLogs.Sum(m => m.MealItems.Sum(i =>
        {
            double grams = ConvertToGrams(i.FoodId, i.Quantity, i.Unit, conversionLookup);
            double multiplier = grams / 100.0;
            return nutrientSelector(i.Food ?? new Food()) * multiplier;
        }));
    }

    #endregion
}