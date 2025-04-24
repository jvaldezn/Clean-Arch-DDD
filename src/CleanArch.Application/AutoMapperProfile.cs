using AutoMapper;
using CleanArch.Application.Products;
using CleanArch.Application.Shared;
using CleanArch.Application.Users;
using CleanArch.Domain.Products;
using CleanArch.Domain.Shared;
using CleanArch.Domain.Users;

namespace CleanArch.Application;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<Log, LogDto>().ReverseMap();
    }
}
