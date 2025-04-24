using CleanArch.Application;
using CleanArch.Application.Products;
using CleanArch.Application.Shared;
using CleanArch.Application.Users;
using CleanArch.Domain.Abstractions;
using CleanArch.Domain.Products;
using CleanArch.Domain.Shared;
using CleanArch.Domain.Users;
using CleanArch.Infrastructure;
using CleanArch.Infrastructure.Context;
using CleanArch.Infrastructure.Messaging.Publisher;
using CleanArch.Infrastructure.Repositories;
using FluentValidation;

namespace CleanArch.API.Extensions;

public static class DependenciesExtensions
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);

        services.AddAutoMapper(typeof(AutoMapperProfile));

        services.AddScoped<IUnitOfWork<AppDbContext>, UnitOfWork<AppDbContext>>();
        services.AddScoped<IUnitOfWork<LogDbContext>, UnitOfWork<LogDbContext>>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<ILogService, LogService>();

        services.AddScoped<IEventPublisher, EventPublisher>();

        services.AddScoped<IValidator<UserDto>, UserDtoValidator>();

        return services;
    }
}
