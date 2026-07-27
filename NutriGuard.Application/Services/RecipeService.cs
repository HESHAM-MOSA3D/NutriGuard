using NutriGuard.Application.DTOs.Recipe;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Application.Interfaces.Services;

namespace NutriGuard.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<RecipeListResponseDto> GetAllAsync(
    string? searchTerm,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default)
    {
        var (items, totalCount) =
            await _recipeRepository.GetPagedAsync(
                searchTerm,
                pageNumber,
                pageSize,
                cancellationToken);

        var dto = items.Select(x => new RecipeDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Servings = x.Servings,
            PreparationTimeMinutes = x.PreparationTimeMinutes
        }).ToList();

        return RecipeListResponseDto.Success(dto, totalCount);
    }




    public async Task<RecipeResponseDto> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        var recipe =
            await _recipeRepository.GetDetailsByIdAsync(
                id,
                cancellationToken);

        if (recipe is null)
        {
            return RecipeResponseDto.Failure("Recipe not found.");
        }

        var dto = new RecipeDetailsDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Instructions = recipe.Instructions,
            Servings = recipe.Servings,
            PreparationTimeMinutes = recipe.PreparationTimeMinutes,

            Ingredients = recipe.RecipeIngredients
                .Select(x => new RecipeIngredientDto
                {
                    FoodId = x.FoodId,
                    FoodName = x.Food.Name,
                    Quantity = x.Quantity,
                    Unit = x.Unit.ToString()
                })
                .ToList()
        };

        return RecipeResponseDto.Success(dto);
    }
}