using NutriGuard.Application.Common.Models;
using NutriGuard.Application.DTOs.Food;

namespace NutriGuard.Application.Interfaces.Services;

public interface IFoodService
{
    Task<FoodListResponseDto> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<FoodResponseDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    //Task<FoodListResponseDto> SearchAsync(
    //    FoodSearchRequestDto request,
    //    CancellationToken cancellationToken = default);



    Task<PagedResult<FoodDto>> SearchFoodsAsync(
        FoodSearchRequestDto request,
        CancellationToken cancellationToken = default);




    Task<FoodListResponseDto> GetByCategoryAsync(
        int categoryId,
        CancellationToken cancellationToken = default);
}