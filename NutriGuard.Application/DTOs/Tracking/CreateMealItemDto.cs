using NutriGuard.Domain.Enums;

namespace NutriGuard.Application.DTOs.Tracking;

public class CreateMealItemDto
{
    public int FoodId { get; set; }
    public decimal Quantity { get; set; }
    public Unit Unit { get; set; }
}
