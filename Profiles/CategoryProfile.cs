using System;
using AutoMapper;
using e_commerce_web_api.DTOs;
using e_commerce_web_api.Models;

namespace e_commerce_web_api.Profiles;

public class CategoryProfile : Profile
{

    public CategoryProfile()
    {
        CreateMap<Category, CategoryReadDto>();
        CreateMap<CreateCategoryDto, Category>();
    }
}
