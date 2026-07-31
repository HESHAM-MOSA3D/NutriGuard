namespace NutriGuard.Domain.Entities;

public class FoodTagAssignment
{
    public int FoodId { get; set; }

    public Food Food { get; set; } = null!;

    public int FoodTagId { get; set; }

    public FoodTag FoodTag { get; set; } = null!;
}