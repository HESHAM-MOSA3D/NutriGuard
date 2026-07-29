using System;
using System.Collections.Generic;

namespace NutriGuard.Application.DTOs.Tracking;

public class MealLogDto
{
    public int Id { get; set; }
    public string MealType { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<MealItemDto> MealItems { get; set; } = new();

    // Calculated totals for this meal
    public double TotalCalories { get; set; }
    public double TotalProtein { get; set; }
    public double TotalCarbs { get; set; }
    public double TotalFat { get; set; }
}
