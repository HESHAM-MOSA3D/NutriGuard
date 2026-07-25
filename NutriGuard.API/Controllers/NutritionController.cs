using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriGuard.Application.Interfaces.Services;
using System.Security.Claims;

namespace NutriGuard.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NutritionController : ControllerBase
{
    private readonly INutritionCalculatorService _nutritionCalculatorService;

    public NutritionController(
        INutritionCalculatorService nutritionCalculatorService)
    {
        _nutritionCalculatorService = nutritionCalculatorService;
    }

    [HttpGet("targets")]
    public async Task<IActionResult> GetTargets(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
}