namespace NutriGuard.Domain.Entities;

public class MealLogItem
{
    public int Id { get; set; }

    public int MealLogId { get; set; }

    public MealLog MealLog { get; set; } = null!;

    public int FoodId { get; set; }

    public Food Food { get; set; } = null!;

    public decimal Quantity { get; set; }

    public Unit Unit { get; set; }
}