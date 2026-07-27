namespace NutriGuard.Application.DTOs.Food;

public sealed class FoodDto
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public int FoodCategoryId { get; init; }

    public string? Category { get; set; }
    public decimal ?Energy { get; init; }

    public decimal ?Protein { get; init; }

    public decimal ?Carbohydrate { get; init; }

    public decimal ?Fat { get; init; }

    public decimal ?Fiber { get; init; }

    public decimal ?Water { get; init; }

    public decimal ?Sodium { get; init; }

    public decimal ?Potassium { get; init; }

    public decimal ?Calcium { get; init; }

    public decimal ?Phosphorus { get; init; }

    public decimal ?Magnesium { get; init; }

    public decimal ?Iron { get; init; }

    public decimal ?Zinc { get; init; }

    public decimal ?Copper { get; init; }

    public decimal ?VitaminC { get; init; }

    public decimal? Thiamin { get; init; }

    public decimal? Riboflavin { get; init; }

    public decimal? VitaminA { get; init; }

    public List<string> Aliases { get; set; } = new();

}