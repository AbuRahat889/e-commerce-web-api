using System;
using e_commerce_web_api.Common;
using e_commerce_web_api.DTOs;
using e_commerce_web_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce_web_api.Controllers;

[ApiController]
[Route("api/v1/products")]
public class ProductControllers : ControllerBase
{

    private readonly IProductService _productService;

    public ProductControllers(IProductService productService)
    {
        _productService = productService;
    }


    // create product 
    [HttpPost]
    public async Task<ActionResult> CreateProductAsync(CreateProductDto data)
    {
        Console.WriteLine($"from controller {data}");

        var result = await _productService.CreateProductAsync(data);

        return Ok(
            ApiResponse<ReadProductDto>.SuccessResponse(200, "Product Create successfully", result)
        );
    }

    // get all products
    [HttpGet]
    public async Task<ActionResult> GetAllProductsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _productService.GetAllProductsAsync(page, pageSize);

        return Ok(ApiResponse<Pagination<ReadProductDto>>.SuccessResponse(200, "Products retrive successfully ", result));
    }


}
