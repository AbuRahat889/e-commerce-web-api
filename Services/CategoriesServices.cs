using AutoMapper;
using e_commerce_web_api.Data;
using e_commerce_web_api.DTOs;
using e_commerce_web_api.Interfaces;
using e_commerce_web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace e_commerce_web_api.Services;


public class CategoriesServices : ICategoryService
{
    private readonly AppDbContext _appDbContext;

    private readonly IMapper _mapper;
    // dependency injection 
    public CategoriesServices(AppDbContext appdbcontext, IMapper mapper)
    {
        _appDbContext = appdbcontext;
        _mapper = mapper;
    }

    // get all category services
    public async Task<Pagination<CategoryReadDto>> GetAllCategory(string searchValue, int page, int pageSize)
    {
        IQueryable<Category> query = _appDbContext.Categories;
        // var query = _appDbContext.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            query = query.Where(c =>
                c.Name != null &&
                EF.Functions.Like(
                    c.Name,
                    $"%{searchValue}%"
                ));
        }

        var totalCount = await query.CountAsync();

        var items = await query.OrderByDescending(c => c.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();


        // var categories = await query.ToListAsync();

        // return _categories.Select(c => new CategoryReadDto
        // {
        //     Id = c.Id,
        //     Name = c.Name,
        //     Description = c.Description,
        //     CreatedAt = c.CreatedAt
        // }).ToList();


        // using auto Mapper _categories => categoryReadDto
        var result = _mapper.Map<List<CategoryReadDto>>(items);
        return new Pagination<CategoryReadDto>
        {
            Data = result,
            Meta = new PaginationMeta
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
            }
        };
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
        return true;
    }
}
