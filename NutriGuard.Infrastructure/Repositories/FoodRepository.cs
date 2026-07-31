using Microsoft.EntityFrameworkCore;
using NutriGuard.Application.Interfaces.Repositories;
using NutriGuard.Domain.Entities;
using NutriGuard.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace NutriGuard.Infrastructure.Repositories;

public sealed class FoodRepository
    : GenericRepository<Food>, IFoodRepository
{
    public FoodRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Food>> GetAllAsync(
       CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.FoodCategory)
            .Include(x => x.Aliases)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Food?> GetByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.FoodCategory)
            .Include(x => x.Aliases)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Food>> GetFoodsByIdsAsync(
        List<int> ids,
        CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        return await _dbSet
            .Include(x => x.FoodCategory)
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Food>> GetByCategoryAsync(
        int categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.FoodCategory)
            .Where(x => x.FoodCategoryId == categoryId)
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }







    public async Task<(List<Food> Items, int TotalCount)> SearchAsync(
    string? searchTerm,
    int? categoryId,
    string sortBy,
    bool sortDescending,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        var term = (searchTerm ?? string.Empty).Trim();

        const string sql = @"
SELECT f.*
FROM ""Foods"" f
WHERE
(
    @p0 = ''
    OR normalize_arabic(f.""Name"") ILIKE '%' || normalize_arabic(@p0) || '%'
    OR EXISTS (
        SELECT 1
        FROM ""FoodAliases"" fa
        WHERE fa.""FoodId"" = f.""Id""
        AND normalize_arabic(fa.""Alias"") ILIKE '%' || normalize_arabic(@p0) || '%'
    )
)";

        IQueryable<Food> query = _context.Foods
            .FromSqlRaw(sql, term)
            .AsNoTracking()
            .Include(f => f.FoodCategory)
            .Include(f => f.Aliases)
            .AsSplitQuery();

        // Optional filter applied as plain LINQ, not raw SQL — avoids
        // the "could not determine data type of parameter" issue entirely.
        if (categoryId.HasValue)
        {
            query = query.Where(f => f.FoodCategoryId == categoryId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, sortBy, sortDescending, term);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    private static IQueryable<Food> ApplySorting(
        IQueryable<Food> query,
        string sortBy,
        bool sortDescending,
        string? searchTerm)
    {
        IOrderedQueryable<Food> ordered;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            ordered = query.OrderByDescending(f => f.Name.ToLower() == term);
        }
        else
        {
            ordered = query.OrderBy(f => 0);
        }

        Expression<Func<Food, object>> keySelector = sortBy switch
        {
            "Energy" => f => f.Energy,
            "Protein" => f => f.Protein,
            "Fat" => f => f.Fat,
            "Carbohydrate" => f => f.Carbohydrate,
            "Category" => f => f.FoodCategory.Name,
            _ => f => f.Name
        };

        query = sortDescending
            ? ordered.ThenByDescending(keySelector)
            : ordered.ThenBy(keySelector);

        return query;
    }
    //public async Task<IReadOnlyList<Food>> SearchAsync(
    // string query,
    // CancellationToken cancellationToken = default)
    //{
    //    query = query.Trim();//.ToLower();

    //    return await _dbSet
    //        .Include(x => x.FoodCategory)
    //        .Include(x => x.Aliases)
    //        .Where(x =>
    //            EF.Functions.ILike(x.Name, $"%{query}%")
    //            ||
    //            x.Aliases.Any(a =>
    //                EF.Functions.ILike(a.Alias, $"%{query}%")))
    //        .AsNoTracking()
    //        .OrderBy(x => x.Name)
    //        .ToListAsync(cancellationToken);
    //}
}