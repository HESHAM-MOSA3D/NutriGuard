using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriGuard.Application.DTOs.Nutrition;
using NutriGuard.Application.Interfaces.Services;
using System.Security.Claims;

namespace NutriGuard.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NutritionController : ControllerBase
{
    private readonly INutritionCalculatorService _nutritionCalculatorService;
    private readonly INutritionRuleEngine _nutritionRuleEngine;

    public NutritionController(
        INutritionCalculatorService nutritionCalculatorService,
        INutritionRuleEngine nutritionRuleEngine)
    {
        _nutritionCalculatorService = nutritionCalculatorService;
        _nutritionRuleEngine = nutritionRuleEngine;
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpGet("targets")]
    public async Task<IActionResult> GetTargets(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var response = await _nutritionCalculatorService
            .CalculateAsync(userId, cancellationToken);

        if (!response.IsSuccess)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost("validate-meal")]
    public async Task<IActionResult> ValidateMeal(
        [FromBody] MealValidationRequestDto request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result = await _nutritionRuleEngine.ValidateMealItemsAsync(
            userId,
            request.MealItems,
            request.Date,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("check-food/{foodId}")]
    public async Task<IActionResult> CheckFoodEligibility(
        int foodId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result = await _nutritionRuleEngine.CheckFoodEligibilityAsync(
            userId,
            foodId,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("eligible-foods")]
    public async Task<IActionResult> FilterEligibleFoods(
        [FromBody] List<int> foodIds,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result = await _nutritionRuleEngine.FilterEligibleFoodsAsync(
            userId,
            foodIds,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("eligible-recipes")]
    public async Task<IActionResult> FilterEligibleRecipes(
        [FromBody] List<int> recipeIds,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result = await _nutritionRuleEngine.FilterEligibleRecipesAsync(
            userId,
            recipeIds,
            cancellationToken);

        return Ok(result);
    }
}