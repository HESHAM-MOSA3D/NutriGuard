using FluentValidation;
using NutriGuard.Application.DTOs.Food;

namespace NutriGuard.Application.Validators.Foods;

public sealed class FoodSearchRequestValidator : AbstractValidator<FoodSearchRequestDto>
{
    private static readonly string[] AllowedSortFields =
    {
        "Name",
        "Energy",
        "Protein",
        "Fat",
        "Carbohydrate",
        "Category"
    };

    public FoodSearchRequestValidator()
    {
        RuleFor(x => x.SearchTerm)
            .MaximumLength(100);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue);

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.SortBy)
            .Must(x => AllowedSortFields.Contains(x))
            .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortFields)}");
    }
}