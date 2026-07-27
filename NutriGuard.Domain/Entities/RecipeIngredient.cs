using NutriGuard.Domain.Enums;

namespace NutriGuard.Domain.Entities;

public class RecipeIngredient
{
    public int Id { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public int FoodId { get; set; }
    public Food Food { get; set; } = null!;

    public decimal Quantity { get; set; }

    public Unit Unit { get; set; }
}