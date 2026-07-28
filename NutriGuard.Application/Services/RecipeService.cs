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

    public async Task<IReadOnlyList<RecipeDto>> GetAllAsync(
     CancellationToken cancellationToken = default)
    {
        var recipes = await _recipeRepository.GetAllAsync(cancellationToken);

        return recipes.Select(recipe => new RecipeDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Servings = recipe.Servings,
            PreparationTimeMinutes = recipe.PreparationTimeMinutes,
            Ingredients = recipe.RecipeIngredients
           .Select(i => new RecipeIngredientDto
            {
              FoodId = i.FoodId,
              FoodName = i.Food.Name,
             Quantity = i.Quantity,
             Unit = i.Unit.ToString()
            })
           .ToList(),
            Aliases = recipe.RecipeAliases
           .Select(a => a.Alias)
           .ToList()

        }).ToList();
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
            Aliases = recipe.RecipeAliases
           .Select(x => x.Alias)
           .ToList(),

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

    public async Task<RecipeListResponseDto> SearchAsync(
    RecipeSearchRequestDto request,
    CancellationToken cancellationToken = default)
    {
        var (recipes, totalCount) =
            await _recipeRepository.SearchAsync(
                request.SearchTerm,
                request.PageNumber,
                request.PageSize,
                request.SortBy,
                request.SortDescending,
                cancellationToken);

        var dto = recipes.Select(recipe => new RecipeDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Servings = recipe.Servings,
            PreparationTimeMinutes = recipe.PreparationTimeMinutes,

            Aliases = recipe.RecipeAliases
                .Select(x => x.Alias)
                .Distinct()
                .ToList(),

            Ingredients = recipe.RecipeIngredients
                .Select(x => new RecipeIngredientDto
                {
                    FoodId = x.FoodId,
                    FoodName = x.Food.Name,
                    Quantity = x.Quantity,
                    Unit = x.Unit.ToString()
                })
                .ToList()

        }).ToList();

        return RecipeListResponseDto.Success(dto, totalCount);
    }



}