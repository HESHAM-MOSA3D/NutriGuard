using NutriGuard.Application.Common.Models;
using NutriGuard.Application.DTOs.Food;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Domain.Entities;
using NutriGuard.Application.Common.Helpers;

namespace NutriGuard.Application.Services;

public sealed class FoodService : IFoodService
{
    private readonly IFoodRepository _foodRepository;

    public FoodService(
        IFoodRepository foodRepository)
    {
        _foodRepository = foodRepository;
    }

    public async Task<FoodListResponseDto> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var foods = await _foodRepository.GetAllAsync(cancellationToken);

        var data = foods.Select(MapToDto);
        

        return FoodListResponseDto.Success(data);
    }

    public async Task<FoodResponseDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var food = await _foodRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (food is null)
        {
            return FoodResponseDto.Failure("Food not found.");
        }

        return FoodResponseDto.Success(MapToDto(food));
    }






    public async Task<PagedResult<FoodDto>> SearchFoodsAsync(
       FoodSearchRequestDto request,
       CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _foodRepository.SearchAsync(
            request.SearchTerm,
            request.CategoryId,
            request.SortBy,
            request.SortDescending,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoItems = items.Select(f => new FoodDto
        {
            Id = f.Id,
            Name = f.Name,
            FoodCategoryId=f.FoodCategoryId,
            Category = f.FoodCategory?.Name ?? "Uncategorized",
            Energy = f.Energy,
            Protein = f.Protein,
            Fat = f.Fat,
            Carbohydrate = f.Carbohydrate,
            Fiber= f.Fiber,
            Water= f.Water,
            Sodium= f.Sodium,
            VitaminA= f.VitaminA,
            VitaminC= f.VitaminC,
            Calcium= f.Calcium,
            Copper= f.Copper,
            Iron= f.Iron,
            Thiamin= f.Thiamin,
            Phosphorus= f.Phosphorus,
            Zinc= f.Zinc,
            Riboflavin= f.Riboflavin,
            Magnesium= f.Magnesium,
            Potassium=f.Potassium,
            

            Aliases = f.Aliases.Select(a => a.Alias).ToList()
        }).ToList();

        return new PagedResult<FoodDto>
        {
            Items = dtoItems,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }







    //public async Task<FoodListResponseDto> SearchAsync(
    //FoodSearchRequestDto request,
    //CancellationToken cancellationToken = default)
    //{
    //    var foods = await _foodRepository.SearchAsync(
    //        request.Query,
    //        cancellationToken);

    //    var data = foods.Select(MapToDto);

    //    return FoodListResponseDto.Success(data);
    //}










    public async Task<FoodListResponseDto> GetByCategoryAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        var foods = await _foodRepository.GetByCategoryAsync(
            categoryId,
            cancellationToken);

        var data = foods.Select(MapToDto);
        

        return FoodListResponseDto.Success(data);
    }




    private static FoodDto MapToDto(Food food)
    {
        return new FoodDto
        {
            Id = food.Id,
            Name = food.Name,

            FoodCategoryId = food.FoodCategoryId,
            Category = food.FoodCategory?.Name,

            Energy = food.Energy,
            Protein = food.Protein,
            Carbohydrate = food.Carbohydrate,
            Fat = food.Fat,
            Fiber = food.Fiber,
            Water = food.Water,

            Sodium = food.Sodium,
            Potassium = food.Potassium,
            Calcium = food.Calcium,
            Phosphorus = food.Phosphorus,
            Magnesium = food.Magnesium,
            Iron = food.Iron,
            Zinc = food.Zinc,
            Copper = food.Copper,

            VitaminA = food.VitaminA,
            VitaminC = food.VitaminC,
            Thiamin = food.Thiamin,
            Riboflavin = food.Riboflavin,

            Aliases = food.Aliases
                .Select(x => x.Alias)
                .ToList()
        };
    }
}
