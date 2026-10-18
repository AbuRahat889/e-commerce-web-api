using AutoMapper;
using e_commerce_web_api.Data;
using e_commerce_web_api.DTOs;
using e_commerce_web_api.Interfaces;
using e_commerce_web_api.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace e_commerce_web_api.Services;


public class CategoriesServices : ICategoryService
{
    private readonly AppDbContext _appDbContext;

    private readonly IMapper _mapper;
    private readonly IDatabase _redisDb;
    // dependency injection 
    public CategoriesServices(AppDbContext appdbcontext, IMapper mapper, IConnectionMultiplexer redis)
    {
        _appDbContext = appdbcontext;
        _mapper = mapper;
        _redisDb = redis.GetDatabase();
    }

    private async Task InvalidateCategoryCache()
    {
        await _redisDb.StringIncrementAsync(
            "categories:cache-version");
    }
    
    // get all category services
    public async Task<Pagination<CategoryReadDto>> GetAllCategory(
        string searchValue,
        int page,
        int pageSize)
    {
        // 1. Get the current cache version
        const string versionKey = "categories:cache-version";

        var versionValue = await _redisDb.StringGetAsync(versionKey);
        var version = versionValue.IsNull
            ? "0"
            : versionValue.ToString();

        // 2. Create a unique key for search and pagination
        var normalizedSearch = searchValue?.Trim().ToLowerInvariant() ?? "";

        var cacheKey =
            $"categories:list:v{version}:search:{normalizedSearch}:page:{page}:size:{pageSize}";

        // 3. Check Redis cache
        var cachedData = await _redisDb.StringGetAsync(cacheKey);

        if (!cachedData.IsNull)
        {
            var cachedResult =
                JsonSerializer.Deserialize<Pagination<CategoryReadDto>>(
                    cachedData.ToString());

            if (cachedResult != null)
            {
                return cachedResult;
            }
        }

        // 4. Cache miss: query PostgreSQL
        IQueryable<Category> query = _appDbContext.Categories;

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            query = query.Where(c =>
                c.Name != null &&
                EF.Functions.ILike(c.Name, $"%{normalizedSearch.Trim()}%"));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = _mapper.Map<List<CategoryReadDto>>(items);

        var response = new Pagination<CategoryReadDto>
        {
            Data = result,
            Meta = new PaginationMeta
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            }
        };

        // 5. Store the result in Redis for 5 minutes
        var json = JsonSerializer.Serialize(response);

        await _redisDb.StringSetAsync(
            cacheKey,
            json,
            TimeSpan.FromMinutes(5));

        return response;
    }

    // get category by id 
    public async Task<CategoryReadDto?> GetCategoryById(Guid categoryId)
    {
        var findCategory = await _appDbContext.Categories.FindAsync(categoryId);
        if (findCategory == null)
        {
            return null;
        }
        return _mapper.Map<CategoryReadDto>(findCategory);
    }
    // create category 
    public async Task<CategoryReadDto> CreateCategory(CreateCategoryDto categorieData)
    {
        var newCategory = _mapper.Map<Category>(categorieData);
        newCategory.Id = Guid.NewGuid();
        newCategory.CreatedAt = DateTime.UtcNow;

        await _appDbContext.Categories.AddAsync(newCategory);
        await _appDbContext.SaveChangesAsync();
        // update cache category version
        await InvalidateCategoryCache();
        return _mapper.Map<CategoryReadDto>(newCategory);
    }


    // update category 
    public async Task<CategoryReadDto?> UPdateCategory(CreateCategoryDto categorieData, Guid categoryId)
    {
        var foundCategory = await _appDbContext.Categories.FindAsync(categoryId);

        if (foundCategory == null)
        {
            return null;
        }
        // categorydata => category
        _mapper.Map(categorieData, foundCategory);
        _appDbContext.Categories.Update(foundCategory);
        await _appDbContext.SaveChangesAsync();
        // update cache category version
        await InvalidateCategoryCache();
        return _mapper.Map<CategoryReadDto>(foundCategory);

    }

    // delete category
    public async Task<bool> DeleteCategory(Guid categoryId)
    {
        var category = await _appDbContext.Categories.FindAsync(categoryId);
        if (category == null)
        {
            return false;
        }
        _appDbContext.Categories.Remove(category);
        await _appDbContext.SaveChangesAsync();
        // update cache category version
        await InvalidateCategoryCache();
        return true;
    }
}
