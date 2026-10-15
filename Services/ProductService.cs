using System;
using AutoMapper;
using e_commerce_web_api.Data;
using e_commerce_web_api.DTOs;
using e_commerce_web_api.Interfaces;
using e_commerce_web_api.Models;
using Microsoft.EntityFrameworkCore;

namespace e_commerce_web_api.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _appdbcontext;
    private readonly IMapper _mapper;

    public ProductService(AppDbContext appDbContext, IMapper mapper)
    {
        _appdbcontext = appDbContext;
        _mapper = mapper;
    }


    public async Task<ReadProductDto> CreateProductAsync(CreateProductDto data)
    {
        Console.WriteLine($"check from service {data}");


        var isCategory = await _appdbcontext.Categories.AnyAsync(c => c.Id == data.CategoryId);
        if (!isCategory)
        {
            throw new KeyNotFoundException("category not found");
        }
        ;
        var newProduct = _mapper.Map<Product>(data);

        _appdbcontext.Products.Add(newProduct);
        await _appdbcontext.SaveChangesAsync();

        return _mapper.Map<ReadProductDto>(newProduct);

    }

    // get all product 
    public async Task<Pagination<ReadProductDto>> GetAllProductsAsync(int page, int pageSize)
    {
        IQueryable<Product> query = _appdbcontext.Products.Include(p => p.Category);
        var totalCount = await query.CountAsync();

        var items = await query.OrderByDescending(c => c.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = _mapper.Map<List<ReadProductDto>>(items);

        return new Pagination<ReadProductDto>
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



}