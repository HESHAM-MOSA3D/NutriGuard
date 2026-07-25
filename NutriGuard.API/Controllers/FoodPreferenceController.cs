using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriGuard.Application.DTOs.FoodPreference;
using NutriGuard.Application.Interfaces.Services;

namespace NutriGuard.API.Controllers;

[ApiController]
[Authorize]
[Route("api/health-profile/{healthProfileId:int}/food-preferences")]
public class FoodPreferenceController : ControllerBase
{
    private readonly IFoodPreferenceService _foodPreferenceService;

    public FoodPreferenceController(IFoodPreferenceService foodPreferenceService)
    {
        _foodPreferenceService = foodPreferenceService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        int healthProfileId,
        CancellationToken cancellationToken)
    {
        var response = await _foodPreferenceService
            .GetByHealthProfileIdAsync(healthProfileId, cancellationToken);

        if (!response.IsSuccess)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        int healthProfileId,
        [FromBody] AddFoodPreferenceRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _foodPreferenceService
            .AddAsync(healthProfileId, request, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(
            nameof(Get),
            new { healthProfileId },
            response);
    }

    [HttpDelete("{foodId:int}")]
    public async Task<IActionResult> Delete(
        int healthProfileId,
        int foodId,
        CancellationToken cancellationToken)
    {
        var response = await _foodPreferenceService
            .RemoveAsync(healthProfileId, foodId, cancellationToken);

        if (!response.IsSuccess)
        {
            return NotFound(response);
        }

        return NoContent();
    }
}