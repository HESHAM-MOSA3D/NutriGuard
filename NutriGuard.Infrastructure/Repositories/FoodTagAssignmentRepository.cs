using Microsoft.EntityFrameworkCore;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Domain.Entities;
using NutriGuard.Infrastructure.Persistence;

namespace NutriGuard.Infrastructure.Repositories;

public class FoodTagAssignmentRepository : IFoodTagAssignmentRepository
{
    private readonly AppDbContext _context;

    public FoodTagAssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FoodTagAssignment>> GetByFoodIdsAsync(
        IEnumerable<int> foodIds,
        CancellationToken cancellationToken = default)
    {
        return await _context.FoodTagAssignments
            .Include(x => x.FoodTag)
            .Where(x => foodIds.Contains(x.FoodId))
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<int, List<int>>> GetFoodTagsMapAsync(
    IEnumerable<int> foodIds,
    CancellationToken cancellationToken = default)
    {
        return await _context.FoodTagAssignments
            .Where(x => foodIds.Contains(x.FoodId))
            .GroupBy(x => x.FoodId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Select(x => x.FoodTagId).ToList(),
                cancellationToken);
    }
}