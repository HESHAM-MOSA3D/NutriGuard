using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NutriGuard.Application.DTOs.Recipe;
using NutriGuard.Application.Interfaces.Services;
using FluentValidation;
using NutriGuard.Application.DTOs.Recipe;

namespace NutriGuard.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;
    private readonly IValidator<RecipeSearchRequestDto> _validator;

    public RecipesController(IRecipeService recipeService, IValidator<RecipeSearchRequestDto> validator)
    {
        _recipeService = recipeService;
        _validator = validator;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _recipeService.GetAllAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var result = await _recipeService.GetByIdAsync(
            id,
            cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }



    //[HttpGet("search")]
    //public async Task<IActionResult> Search(
    //[FromQuery] string name,
    //CancellationToken cancellationToken)
    //{
    //    var result = await _recipeService.SearchAsync(
    //        name,
    //        cancellationToken);

    //    return Ok(result);
    //}






    [HttpGet("search")]
    public async Task<IActionResult> Search(
     [FromQuery] RecipeSearchRequestDto request,
     CancellationToken cancellationToken)
    {
        var validationResult =
            await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors.Select(e => new
            {
                Field = e.PropertyName,
                Error = e.ErrorMessage
            }));
        }

        var result =
            await _recipeService.SearchAsync(request, cancellationToken);

        return Ok(result);
    }

}