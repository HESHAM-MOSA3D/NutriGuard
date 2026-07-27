namespace NutriGuard.Domain.Entities;

public class FoodCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Food> Foods { get; set; }
        = new List<Food>();
}