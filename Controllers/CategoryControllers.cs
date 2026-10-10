using e_commerce_web_api.DTOs;
using e_commerce_web_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_api.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoryController : ControllerBase
    {
        // category dependency injection 
        private readonly ICategoryService _categoryservice;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryservice = categoryService;
        }
        // end dependency constructor

        // for get request 
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] string searchValue = "", [FromQuery] int page = 1, [FromQuery] int PageSize = 10)
        {
            var categoryList = await _categoryservice.GetAllCategory(searchValue, page, PageSize);

            return Ok(ApiResponse<Pagination<CategoryReadDto>>.SuccessResponse(200, "Category retrived successfully", categoryList));
        }

        // get category by id 
        [HttpGet("{categoryId:guid}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid categoryId)
        {
            var result = await _categoryservice.GetCategoryById(categoryId);

            if (result == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Validation Error", 404, new List<string> { "Category is not found with this id!" }));
            }
            return Ok(ApiResponse<CategoryReadDto>.SuccessResponse(200, "Category retrived successfully", result));
        }


        // create ctegories
        [HttpPost]
        public async Task<IActionResult> CreateCategories([FromBody] CreateCategoryDto categorieData)
        {
            var result = await _categoryservice.CreateCategory(categorieData);
            return Ok(ApiResponse<CategoryReadDto>.SuccessResponse(201, "Category create successfully", result));
        }

        // upadate categories by id 
        [HttpPut("{categoryId:guid}")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] CreateCategoryDto categorieData)
        {
            var category = await _categoryservice.UPdateCategory(categorieData, categoryId);
            if (category == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Validation Error", 404, new List<string> { "Category is not found with this id!" }));
            }
            return Ok(ApiResponse<CategoryReadDto>.SuccessResponse(200, "Category update successfully", category));
        }

        // delete category by id
        [HttpDelete("{categoryId:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            var foundCategory = await _categoryservice.DeleteCategory(categoryId);
            if (!foundCategory)
            {
                return NotFound(ApiResponse<object>.ErrorResponse("Validation Error", 404, new List<string> { "Category is not found with this id!" }));
            }
            return Ok(ApiResponse<object?>.SuccessResponse(200, "Category deleted successfully", null));
        }

    }
}

