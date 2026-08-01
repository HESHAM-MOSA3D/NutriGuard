using System;
using System.Collections.Generic;

namespace NutriGuard.Application.DTOs.Tracking;

public class DailySummaryDto
{
    public DateOnly Date { get; set; }

    // Calories Tracking
    public double CaloriesTarget { get; set; }
    public double CaloriesConsumed { get; set; }
    public double CaloriesRemaining { get; set; }
    public double CaloriesExceeded { get; set; }

    // Macronutrients Tracking
    public double ProteinTargetGrams { get; set; }
    public double ProteinConsumedGrams { get; set; }
    public double ProteinRemainingGrams { get; set; }

    public double CarbsTargetGrams { get; set; }
    public double CarbsConsumedGrams { get; set; }
    public double CarbsRemainingGrams { get; set; }
    public double CarbsExceeded { get; set; }

    public double FatTargetGrams { get; set; }
    public double FatConsumedGrams { get; set; }
    public double FatRemainingGrams { get; set; }
    public double FatExceeded { get; set; }

    // Water Tracking
    public double WaterTargetMl { get; set; }
    public double WaterConsumedMl { get; set; }
    public double WaterRemainingMl { get; set; }

    // Weight Tracking
    public double? CurrentWeightKg { get; set; }

    // Status Flags
    public bool IsCaloriesExceeded { get; set; }
    public bool IsCarbsExceeded { get; set; }
    public bool IsFatExceeded { get; set; }
    public bool IsProteinCompleted { get; set; }
    public bool IsWaterCompleted { get; set; }

    // Completion Percentages
    public CompletionDto Completion { get; set; } = new();

    // Daily Nutrition Score
    public int DailyNutritionScore { get; set; }

    // Summary Message
    public string SummaryMessage { get; set; } = string.Empty;

    // Detailed Meals logged today
    public List<MealLogDto> Meals { get; set; } = new();
}

public class CompletionDto
{
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
    public double Water { get; set; }
}
