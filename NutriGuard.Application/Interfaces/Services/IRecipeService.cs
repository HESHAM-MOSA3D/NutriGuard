using NutriGuard.Application.DTOs.Recipe;

namespace NutriGuard.Application.Interfaces.Services;

public interface IRecipeService
{
    Task<RecipeListResponseDto> GetAllAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<RecipeResponseDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}