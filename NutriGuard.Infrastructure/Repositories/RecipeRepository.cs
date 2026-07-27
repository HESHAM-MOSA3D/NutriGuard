using Microsoft.EntityFrameworkCore;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Domain.Entities;
using NutriGuard.Infrastructure.Persistence;
using NutriGuard.Infrastructure.Repositories;


namespace NutriGuard.Infrastructure.Repositories;

public class RecipeRepository
    : GenericRepository<Recipe>, IRecipeRepository
{
    private readonly AppDbContext _context;

    public RecipeRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<(List<Recipe> Items, int TotalCount)> GetPagedAsync(
    string? searchTerm,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default)
    {
        var query = _context.Recipes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x =>
     EF.Functions.ILike(x.Name, $"%{searchTerm}%") ||
     EF.Functions.ILike(x.Description, $"%{searchTerm}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }



    public async Task<Recipe?> GetDetailsByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        return await _context.Recipes
            .AsNoTracking()
            .Include(x => x.RecipeIngredients)
                .ThenInclude(x => x.Food)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}