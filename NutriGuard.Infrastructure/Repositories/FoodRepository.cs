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
        var query = _context.Foods
     .AsNoTracking()
     .Include(f => f.FoodCategory)
     .Include(f => f.Aliases)
     .AsSplitQuery()
     .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(f => f.FoodCategoryId == categoryId.Value);
        }
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();

            query = query.Where(f =>
                EF.Functions.ILike(f.Name, $"%{term}%") ||
                f.Aliases.Any(a => EF.Functions.ILike(a.Alias, $"%{term}%")));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, sortBy, sortDescending, searchTerm);

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
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.OrderByDescending(f => f.Name.ToLower() == searchTerm.Trim().ToLower());
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
            ? query.OrderBy(f => 0).ThenByDescending(keySelector)
            : query.OrderBy(f => 0).ThenBy(keySelector);

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