using NutriGuard.Domain.Entities;

public class FoodUnitConversion
{
    public int Id { get; set; }

    public int FoodId { get; set; }
    public Food Food { get; set; } = null!;

    public Unit Unit { get; set; }

    public double GramsPerUnit { get; set; }

    public bool IsDefault { get; set; }
}