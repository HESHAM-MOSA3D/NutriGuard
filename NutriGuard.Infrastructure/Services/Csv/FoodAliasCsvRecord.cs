namespace NutriGuard.Infrastructure.Csv;

public class FoodAliasCsvRecord
{
    public string Food { get; set; } = string.Empty;

    public string Alias { get; set; } = string.Empty;

    public int Language { get; set; }
}