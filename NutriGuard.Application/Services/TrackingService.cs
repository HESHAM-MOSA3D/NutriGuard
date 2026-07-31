using NutriGuard.Application.DTOs.Tracking;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Domain.Entities;
using NutriGuard.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NutriGuard.Application.Services;

public class TrackingService : ITrackingService
{
    private readonly IMealLogRepository _mealLogRepository;
    private readonly IWaterLogRepository _waterLogRepository;
    private readonly IWeightLogRepository _weightLogRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly INutritionCalculatorService _nutritionCalculator;
    private readonly IFoodUnitConversionRepository _foodUnitConversionRepository;
    private readonly INutritionRuleEngine _nutritionRuleEngine;
    private readonly IHealthProfileService _healthProfileService;

    public TrackingService(
    IMealLogRepository mealLogRepository,
    IWaterLogRepository waterLogRepository,
    IWeightLogRepository weightLogRepository,
    IFoodRepository foodRepository,
    INutritionCalculatorService nutritionCalculator,
    IFoodUnitConversionRepository foodUnitConversionRepository,
    INutritionRuleEngine nutritionRuleEngine,
    IHealthProfileService healthProfileService)
    {
        _mealLogRepository = mealLogRepository;
        _waterLogRepository = waterLogRepository;
        _weightLogRepository = weightLogRepository;
        _foodRepository = foodRepository;
        _nutritionCalculator = nutritionCalculator;
        _foodUnitConversionRepository = foodUnitConversionRepository;

        _nutritionRuleEngine = nutritionRuleEngine;
        _healthProfileService = healthProfileService;
    }
    public async Task<MealLogResponseDto> LogMealAsync(string userId, LogMealRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.MealItems == null || !request.MealItems.Any())
        {
            return MealLogResponseDto.Failure("Meal must contain at least one item.");
        }

        if (!Enum.IsDefined(typeof(MealType), request.MealType))
        {
            return MealLogResponseDto.Failure("Invalid meal type.");
        }

        foreach (var item in request.MealItems)
        {
            if (item.Quantity <= 0)
            {
                return MealLogResponseDto.Failure("Quantity must be greater than zero.");
            }
        }

        var mealLog = new MealLog
        {
            UserId = userId,
            MealType = request.MealType,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow
        };

        var foodIds = request.MealItems.Select(x => x.FoodId).Distinct().ToList();
        var foods = await _foodRepository.GetFoodsByIdsAsync(foodIds, cancellationToken);

        var profileResult = await _healthProfileService.GetAsync(userId, cancellationToken);

        if (!profileResult.IsSuccess || profileResult.Data is null)
        {
            return MealLogResponseDto.Failure("Health profile not found.");
        }

       

     



        var conversions = await _foodUnitConversionRepository
           .GetByFoodIdsAsync(foodIds, cancellationToken);

        var conversionLookup = conversions
            .GroupBy(x => x.FoodId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.Unit).ToHashSet());
        var foodDict = foods.ToDictionary(f => f.Id);

        foreach (var item in request.MealItems)
        {
            if (!foodDict.ContainsKey(item.FoodId))
            {
                return MealLogResponseDto.Failure($"Food with ID {item.FoodId} not found.");
            }

            if (!conversionLookup.TryGetValue(item.FoodId, out var allowedUnits))
            {
                return MealLogResponseDto.Failure(
                    $"No unit conversions found for Food ID {item.FoodId}.");
            }

            if (!allowedUnits.Contains(item.Unit))
            {
                return MealLogResponseDto.Failure(
                    $"Unit '{item.Unit}' is not valid for food '{foodDict[item.FoodId].Name}'.");
            }

            mealLog.MealItems.Add(new MealItem
            {
                FoodId = item.FoodId,
                Quantity = item.Quantity,
                Unit = item.Unit
            });
        }

        await _mealLogRepository.AddAsync(mealLog, cancellationToken);
        await _mealLogRepository.SaveChangesAsync(cancellationToken);

        // Map to Dto
        var logFromDb = await _mealLogRepository.GetMealLogByIdWithItemsAsync(mealLog.Id, cancellationToken);
        var dto = await MapToMealLogDtoAsync(
            logFromDb!,
            cancellationToken);
        return MealLogResponseDto.Success(dto);
    }

    public async Task<WaterLogResponseDto> LogWaterAsync(string userId, LogWaterRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.AmountInMl <= 0)
        {
            return WaterLogResponseDto.Failure("Water amount must be greater than zero.");
        }

        if (request.AmountInMl > 10000)
        {
            return WaterLogResponseDto.Failure("Water amount exceeds the maximum allowed.");
        }

        var waterLog = new WaterLog
        {
            UserId = userId,
            AmountInMl = request.AmountInMl,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow
        };

        await _waterLogRepository.AddAsync(waterLog, cancellationToken);
        await _waterLogRepository.SaveChangesAsync(cancellationToken);

        return WaterLogResponseDto.Success(new WaterLogDto
        {
            Id = waterLog.Id,
            AmountInMl = waterLog.AmountInMl,
            Date = waterLog.Date,
            CreatedAt = waterLog.CreatedAt
        });
    }

    public async Task<WeightLogResponseDto> LogWeightAsync(string userId, LogWeightRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.Weight <= 0)
        {
            return WeightLogResponseDto.Failure("Weight must be greater than zero.");
        }

        if (request.Weight > 500)
        {
            return WeightLogResponseDto.Failure("Weight exceeds the maximum allowed.");
        }

        var weightLog = new WeightLog
        {
            UserId = userId,
            Weight = request.Weight,
            Date = request.Date,
            CreatedAt = DateTime.UtcNow
        };

        await _weightLogRepository.AddAsync(weightLog, cancellationToken);
        await _weightLogRepository.SaveChangesAsync(cancellationToken);

        return WeightLogResponseDto.Success(new WeightLogDto
        {
            Id = weightLog.Id,
            Weight = weightLog.Weight,
            Date = weightLog.Date,
            CreatedAt = weightLog.CreatedAt
        });
    }

    public async Task<DailySummaryDto> GetDailySummaryAsync(string userId, DateOnly date, CancellationToken cancellationToken = default)
    {
        var mealLogs = await _mealLogRepository.GetUserMealLogsByDateAsync(userId, date, cancellationToken);
        var waterTotal = await _waterLogRepository.GetTotalWaterIntakeByDateAsync(userId, date, cancellationToken);
        var targetsResult = await _nutritionCalculator.CalculateAsync(userId, cancellationToken);
        var targets = targetsResult.Data;

        var summary = new DailySummaryDto
        {
            Date = date,
            WaterConsumedMl = waterTotal
        };

        if (targets != null)
        {
            summary.CaloriesTarget = targets.DailyCalories;
            summary.ProteinTargetGrams = targets.ProteinGrams;
            summary.CarbsTargetGrams = targets.CarbsGrams;
            summary.FatTargetGrams = targets.FatGrams;
            summary.WaterTargetMl = 2000;
        }

        foreach (var log in mealLogs)
        {
            var logDto = await MapToMealLogDtoAsync(
                log,
                cancellationToken); summary.Meals.Add(logDto);

            summary.CaloriesConsumed += logDto.TotalCalories;
            summary.ProteinConsumedGrams += logDto.TotalProtein;
            summary.CarbsConsumedGrams += logDto.TotalCarbs;
            summary.FatConsumedGrams += logDto.TotalFat;
        }

        summary.CaloriesRemaining = Math.Max(0, summary.CaloriesTarget - summary.CaloriesConsumed);
        summary.ProteinRemainingGrams = Math.Max(0, summary.ProteinTargetGrams - summary.ProteinConsumedGrams);
        summary.CarbsRemainingGrams = Math.Max(0, summary.CarbsTargetGrams - summary.CarbsConsumedGrams);
        summary.FatRemainingGrams = Math.Max(0, summary.FatTargetGrams - summary.FatConsumedGrams);
        summary.WaterRemainingMl = Math.Max(0, summary.WaterTargetMl - summary.WaterConsumedMl);

        var latestWeight = await _weightLogRepository.GetLatestUserWeightLogAsync(userId, cancellationToken);
        if (latestWeight != null)
        {
            summary.CurrentWeightKg = latestWeight.Weight;
        }

        return summary;
    }

    public async Task<IEnumerable<DailySummaryDto>> GetDailyHistoryAsync(string userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        // 1. Fetch static Targets once
        var targetsResult = await _nutritionCalculator.CalculateAsync(userId, cancellationToken);
        var targets = targetsResult.Data;

        // 2. Fetch latest weight once
        var latestWeight = await _weightLogRepository.GetLatestUserWeightLogAsync(userId, cancellationToken);
        double? currentWeight = latestWeight?.Weight;

        // 3. Fetch all meal logs in the range in one query (Eager Loaded)
        var allMealLogs = await _mealLogRepository.GetUserMealLogsInDateRangeAsync(userId, startDate, endDate, cancellationToken);
        
        // Group meal logs by date in memory
        var mealLogsByDate = allMealLogs.GroupBy(x => x.Date).ToDictionary(g => g.Key, g => g.ToList());

        // 4. Fetch all water logs in the range in one query
        var allWaterLogs = await _waterLogRepository.GetUserWaterLogsInDateRangeAsync(userId, startDate, endDate, cancellationToken);

        // Group water logs by date and calculate sum in memory
        var waterTotalByDate = allWaterLogs
            .GroupBy(x => x.Date)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.AmountInMl));

        var summaries = new List<DailySummaryDto>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var waterTotal = waterTotalByDate.ContainsKey(date) ? waterTotalByDate[date] : 0.0;
            var mealLogs = mealLogsByDate.ContainsKey(date) ? mealLogsByDate[date] : new List<MealLog>();

            var summary = new DailySummaryDto
            {
                Date = date,
                WaterConsumedMl = waterTotal,
                CurrentWeightKg = currentWeight
            };

            if (targets != null)
            {
                summary.CaloriesTarget = targets.DailyCalories;
                summary.ProteinTargetGrams = targets.ProteinGrams;
                summary.CarbsTargetGrams = targets.CarbsGrams;
                summary.FatTargetGrams = targets.FatGrams;
                summary.WaterTargetMl = 2000;
            }

            foreach (var log in mealLogs)
            {
                var logDto = await MapToMealLogDtoAsync(
                    log,
                    cancellationToken); summary.Meals.Add(logDto);

                summary.CaloriesConsumed += logDto.TotalCalories;
                summary.ProteinConsumedGrams += logDto.TotalProtein;
                summary.CarbsConsumedGrams += logDto.TotalCarbs;
                summary.FatConsumedGrams += logDto.TotalFat;
            }

            summary.CaloriesRemaining = Math.Max(0, summary.CaloriesTarget - summary.CaloriesConsumed);
            summary.ProteinRemainingGrams = Math.Max(0, summary.ProteinTargetGrams - summary.ProteinConsumedGrams);
            summary.CarbsRemainingGrams = Math.Max(0, summary.CarbsTargetGrams - summary.CarbsConsumedGrams);
            summary.FatRemainingGrams = Math.Max(0, summary.FatTargetGrams - summary.FatConsumedGrams);
            summary.WaterRemainingMl = Math.Max(0, summary.WaterTargetMl - summary.WaterConsumedMl);

            summaries.Add(summary);
        }

        return summaries;
    }

    public async Task<BaseResponse> DeleteMealAsync(string userId, int mealLogId, CancellationToken cancellationToken = default)
    {
        var meal = await _mealLogRepository.GetByIdAsync(mealLogId, cancellationToken);
        if (meal == null)
        {
            return BaseResponse.Failure("Meal log not found.");
        }

        if (meal.UserId != userId)
        {
            return BaseResponse.Failure("Unauthorized.");
        }

        _mealLogRepository.Delete(meal);
        await _mealLogRepository.SaveChangesAsync(cancellationToken);
        return BaseResponse.Success("Meal log deleted successfully.");
    }

    public async Task<BaseResponse> DeleteWaterAsync(string userId, int waterLogId, CancellationToken cancellationToken = default)
    {
        var water = await _waterLogRepository.GetByIdAsync(waterLogId, cancellationToken);
        if (water == null)
        {
            return BaseResponse.Failure("Water log not found.");
        }

        if (water.UserId != userId)
        {
            return BaseResponse.Failure("Unauthorized.");
        }

        _waterLogRepository.Delete(water);
        await _waterLogRepository.SaveChangesAsync(cancellationToken);
        return BaseResponse.Success("Water log deleted successfully.");
    }

    public async Task<BaseResponse> DeleteWeightAsync(string userId, int weightLogId, CancellationToken cancellationToken = default)
    {
        var weight = await _weightLogRepository.GetByIdAsync(weightLogId, cancellationToken);
        if (weight == null)
        {
            return BaseResponse.Failure("Weight log not found.");
        }

        if (weight.UserId != userId)
        {
            return BaseResponse.Failure("Unauthorized.");
        }

        _weightLogRepository.Delete(weight);
        await _weightLogRepository.SaveChangesAsync(cancellationToken);
        return BaseResponse.Success("Weight log deleted successfully.");
    }

    private async Task<MealLogDto> MapToMealLogDtoAsync(
    MealLog log,
    CancellationToken cancellationToken = default)
    {
        var dto = new MealLogDto
        {
            Id = log.Id,
            MealType = log.MealType.ToString(),
            Date = log.Date,
            CreatedAt = log.CreatedAt,
            MealItems = new List<MealItemDto>()
        };

        foreach (var item in log.MealItems)
        {
            double grams = await ConvertToGramsAsync(
                           item.FoodId,
                           item.Quantity,
                           item.Unit,
                           cancellationToken);

            double multiplier = grams / 100.0;

            var itemDto = new MealItemDto
            {
                Id = item.Id,
                FoodId = item.FoodId,
                FoodName = item.Food?.Name ?? string.Empty,
                Quantity = item.Quantity,
                Unit = item.Unit.ToString(),
                Calories = (double)(item.Food?.Energy ?? 0) * multiplier,
                Protein = (double)(item.Food?.Protein ?? 0) * multiplier,
                Carbs = (double)(item.Food?.Carbohydrate ?? 0) * multiplier,
                Fat = (double)(item.Food?.Fat ?? 0) * multiplier
            };
            dto.MealItems.Add(itemDto);

            dto.TotalCalories += itemDto.Calories;
            dto.TotalProtein += itemDto.Protein;
            dto.TotalCarbs += itemDto.Carbs;
            dto.TotalFat += itemDto.Fat;
        }

        return dto;
    }

    

    private async Task<double> ConvertToGramsAsync(
     int foodId,
     decimal quantity,
     Unit unit,
     CancellationToken cancellationToken = default)
    {
        var conversion = await _foodUnitConversionRepository.GetConversionAsync(
            foodId,
            unit,
            cancellationToken);

        if (conversion != null)
        {
            return (double)quantity * conversion.GramsPerUnit;
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
}
