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

        searchTerm = string.IsNullOrWhiteSpace(searchTerm) ? string.Empty : searchTerm.Trim();

        var orderColumn = sortBy?.ToLower() switch
        {
            "time" => "\"PreparationTimeMinutes\"",
            "servings" => "\"Servings\"",
            _ => "\"Name\""
        };
        var orderDirection = sortDescending ? "DESC" : "ASC";

        var countSql = @"
        SELECT COUNT(DISTINCT r.""Id"") 
        FROM ""Recipes"" r
        LEFT JOIN ""RecipeAliases"" ra ON ra.""RecipeId"" = r.""Id""
        WHERE (@p0 = '' 
            OR normalize_arabic(r.""Name"") ILIKE '%' || normalize_arabic(@p0) || '%'
            OR normalize_arabic(ra.""Alias"") ILIKE '%' || normalize_arabic(@p0) || '%')";

        int totalCount;
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = countSql;
            var parameter = command.CreateParameter();
            parameter.ParameterName = "p0";
            parameter.Value = searchTerm;
            command.Parameters.Add(parameter);

            await _context.Database.OpenConnectionAsync(cancellationToken);
            totalCount = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
        }

        var sql = $@"
        SELECT DISTINCT r.* 
        FROM ""Recipes"" r
        LEFT JOIN ""RecipeAliases"" ra ON ra.""RecipeId"" = r.""Id""
        WHERE (@p0 = '' 
            OR normalize_arabic(r.""Name"") ILIKE '%' || normalize_arabic(@p0) || '%'
            OR normalize_arabic(ra.""Alias"") ILIKE '%' || normalize_arabic(@p0) || '%')
        ORDER BY r.{orderColumn} {orderDirection}
        LIMIT @p1 OFFSET @p2";

        int offset = (pageNumber - 1) * pageSize;

        var recipes = await _context.Recipes
            .FromSqlRaw($@"
            SELECT DISTINCT r.* 
            FROM ""Recipes"" r
            LEFT JOIN ""RecipeAliases"" ra ON ra.""RecipeId"" = r.""Id""
            WHERE ('{searchTerm}' = '' 
                OR normalize_arabic(r.""Name"") ILIKE '%' || normalize_arabic('{searchTerm}') || '%'
                OR normalize_arabic(ra.""Alias"") ILIKE '%' || normalize_arabic('{searchTerm}') || '%')
            ORDER BY r.{orderColumn} {orderDirection}
            LIMIT {pageSize} OFFSET {offset}")
            .AsNoTracking()
            .Include(x => x.RecipeAliases)
            .Include(x => x.RecipeIngredients)
                .ThenInclude(x => x.Food)
            .ToListAsync(cancellationToken);

        return (recipes, totalCount);
    }
}