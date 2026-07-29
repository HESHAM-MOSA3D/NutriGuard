using NutriGuard.Application.DTOs.Recipe;

namespace NutriGuard.Application.Interfaces.Services;

public interface IRecipeService
{
  

    Task<RecipeResponseDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);


   

    Task<IReadOnlyList<RecipeDto>> GetAllAsync(
    CancellationToken cancellationToken = default);

    Task<RecipeListResponseDto> SearchAsync(
        RecipeSearchRequestDto request,
        CancellationToken cancellationToken = default);

}