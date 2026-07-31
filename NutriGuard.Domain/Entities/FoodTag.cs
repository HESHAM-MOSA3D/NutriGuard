namespace NutriGuard.Domain.Entities;

public class FoodTag
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<FoodTagAssignment> FoodTagAssignments { get; set; }
        = new List<FoodTagAssignment>();
}