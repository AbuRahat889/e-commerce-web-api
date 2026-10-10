using System;
using e_commerce_web_api.Controllers;
using e_commerce_web_api.DTOs;

namespace e_commerce_web_api.Interfaces;

public interface ICategoryService
{
    Task<Pagination<CategoryReadDto>> GetAllCategory(string searchValue, int page, int limit);
    Task<CategoryReadDto?> GetCategoryById(Guid categoryId);
    Task<CategoryReadDto> CreateCategory(CreateCategoryDto categorieData);
    Task<CategoryReadDto?> UPdateCategory(CreateCategoryDto categorieData, Guid categoryId);
    Task<bool> DeleteCategory(Guid categoryId);

}
