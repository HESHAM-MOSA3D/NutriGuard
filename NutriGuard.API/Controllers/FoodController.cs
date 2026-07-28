using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NutriGuard.Application.DTOs.Food;
using NutriGuard.Application.Interfaces.Services;
using NutriGuard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace NutriGuard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class FoodsController : ControllerBase
{
    private readonly IFoodService _foodService;
    private readonly IValidator<FoodSearchRequestDto> _validator;

    public FoodsController(IFoodService foodService, IValidator<FoodSearchRequestDto> validator)
    {
        _foodService = foodService;
        _validator = validator;
    }

   
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _foodService.GetAllAsync(cancellationToken);

        return Ok(result);
    }

    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _foodService.GetByIdAsync(
            id,
            cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }






   

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] FoodSearchRequestDto request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new
            {
                Field = e.PropertyName,
                Error = e.ErrorMessage
            }));
        }

        var result = await _foodService.SearchFoodsAsync(request, cancellationToken);
        return Ok(result);
    }


    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(
    [FromServices] AppDbContext context,
    CancellationToken cancellationToken)
    {
        var categories = await context.FoodCategories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.Name
            })
            .ToListAsync(cancellationToken);

        return Ok(categories);
    }

}