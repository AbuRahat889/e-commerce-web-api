using System;
using e_commerce_web_api.DTOs;
namespace e_commerce_web_api.Interfaces;

public interface IProductService
{
    Task<ReadProductDto> CreateProductAsync(CreateProductDto data);

    // Task<IEnumerable<ReadProductDto>> GetAllProductsAsync();

    // Task<ReadProductDto?> GetProductByIdAsync(Guid id);
}
