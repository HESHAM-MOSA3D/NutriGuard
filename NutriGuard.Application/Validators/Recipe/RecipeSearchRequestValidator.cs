using FluentValidation;
using NutriGuard.Application.DTOs.Recipe;

namespace NutriGuard.Application.Validators;

public class RecipeSearchRequestValidator
    : AbstractValidator<RecipeSearchRequestDto>
{
    public RecipeSearchRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}