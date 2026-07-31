using Microsoft.EntityFrameworkCore;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Domain.Entities;
using NutriGuard.Infrastructure.Persistence;
using NutriGuard.Infrastructure.Repositories;
using System.Linq.Expressions;


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
            .Include(x => x.RecipeAliases)
            .Include(x => x.RecipeIngredients)
                .ThenInclude(x => x.Food)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }






    public async Task<IReadOnlyList<Recipe>> GetAllAsync(
     CancellationToken cancellationToken = default)
    {
        return await _context.Recipes
            .AsNoTracking()
            .Include(x => x.RecipeAliases)
            .Include(x => x.RecipeIngredients)
                .ThenInclude(x => x.Food)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Recipe>, int)> SearchAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        string? sortBy,
        bool sortDescending,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        var term = (searchTerm ?? string.Empty).Trim();

        const string sql = @"
SELECT r.*
FROM ""Recipes"" r
WHERE
(
    @p0 = ''
    OR normalize_arabic(r.""Name"") ILIKE '%' || normalize_arabic(@p0) || '%'
    OR EXISTS (
        SELECT 1
        FROM ""RecipeAliases"" ra
        WHERE ra.""RecipeId"" = r.""Id""
        AND normalize_arabic(ra.""Alias"") ILIKE '%' || normalize_arabic(@p0) || '%'
    )
)";

        var query = _context.Recipes
            .FromSqlRaw(sql, term)
            .AsNoTracking()
            .Include(x => x.RecipeAliases)
            .Include(x => x.RecipeIngredients)
                .ThenInclude(x => x.Food)
            .AsSplitQuery()
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, sortBy, sortDescending, term);

        var recipes = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (recipes, totalCount);
    }

    private static IQueryable<Recipe> ApplySorting(
        IQueryable<Recipe> query,
        string? sortBy,
        bool sortDescending,
        string? searchTerm)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.OrderByDescending(r => r.Name.ToLower() == term);
        }
        else
        {
            query = query.OrderBy(r => 0);
        }

        Expression<Func<Recipe, object>> keySelector = sortBy?.ToLower() switch
        {
            "time" => r => r.PreparationTimeMinutes,
            "servings" => r => r.Servings,
            _ => r => r.Name
        };

        var ordered = (IOrderedQueryable<Recipe>)query;
        query = sortDescending ? ordered.ThenByDescending(keySelector) : ordered.ThenBy(keySelector);

        return query;
    }


    //    public async Task<(IReadOnlyList<Recipe>, int)> SearchAsync(
    //    string? searchTerm,
    //    int pageNumber,
    //    int pageSize,
    //    string? sortBy,
    //    bool sortDescending,
    //    CancellationToken cancellationToken = default)
    //    {
    //        if (pageNumber <= 0) pageNumber = 1;
    //        if (pageSize <= 0) pageSize = 10;

    //        searchTerm ??= string.Empty;

    //        var orderColumn = sortBy?.ToLower() switch
    //        {
    //            "time" => "\"PreparationTimeMinutes\"",
    //            "servings" => "\"Servings\"",
    //            _ => "\"Name\""
    //        };

    //        var orderDirection = sortDescending ? "DESC" : "ASC";

    //        var sql = $@"
    //SELECT DISTINCT r.*
    //FROM ""Recipes"" r
    //LEFT JOIN ""RecipeAliases"" ra
    //    ON ra.""RecipeId"" = r.""Id""
    //WHERE
    //(
    //    @p0 = ''
    //    OR normalize_arabic(r.""Name"")
    //        ILIKE '%' || normalize_arabic(@p0) || '%'
    //    OR normalize_arabic(ra.""Alias"")
    //        ILIKE '%' || normalize_arabic(@p0) || '%'
    //)
    //ORDER BY {orderColumn} {orderDirection}";

    //        var query = _context.Recipes
    //            .FromSqlRaw(sql, searchTerm)
    //            .AsNoTracking()
    //            .Include(x => x.RecipeAliases)
    //            .Include(x => x.RecipeIngredients)
    //                .ThenInclude(x => x.Food);

    //        var totalCount = await query.CountAsync(cancellationToken);

    //        var recipes = await query
    //            .Skip((pageNumber - 1) * pageSize)
    //            .Take(pageSize)
    //            .ToListAsync(cancellationToken);

    //        return (recipes, totalCount);
    //    }
}