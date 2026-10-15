using System;
using e_commerce_web_api.DTOs;
namespace e_commerce_web_api.Interfaces;

public interface IProductService
{
    Task<ReadProductDto> CreateProductAsync(CreateProductDto data);

    Task<Pagination<ReadProductDto>> GetAllProductsAsync(int page, int pageSize);

    // Task<ReadProductDto?> GetProductByIdAsync(Guid id);
}
