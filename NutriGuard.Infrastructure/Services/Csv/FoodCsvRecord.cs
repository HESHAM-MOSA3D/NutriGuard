using CsvHelper.Configuration.Attributes;

namespace NutriGuard.Infrastructure.Csv;

public class FoodCsvRecord
{
    public string Name { get; set; } = string.Empty;

    public decimal ?RefusePercentage { get; set; }

    public decimal ?Water { get; set; }

    public decimal ?Energy { get; set; }

    public decimal ?Protein { get; set; }

    public decimal ?Fat { get; set; }

    public decimal ?Ash { get; set; }

    public decimal ?Fiber { get; set; }

    public decimal ?Carbohydrate { get; set; }

    public decimal ?Sodium { get; set; }

    public decimal ?Potassium { get; set; }

    public decimal ?Calcium { get; set; }

    public decimal ?Phosphorus { get; set; }

    public decimal ?Magnesium { get; set; }

    public decimal ?Iron { get; set; }

    public decimal ?Zinc { get; set; }

    public decimal ?Copper { get; set; }

    public decimal? VitaminA { get; set; }

    public decimal ?VitaminC { get; set; }

    public decimal ?Thiamin { get; set; }

    public decimal? Riboflavin { get; set; }
}