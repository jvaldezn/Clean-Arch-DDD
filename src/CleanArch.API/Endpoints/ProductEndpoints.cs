using CleanArch.API.Extensions;
using CleanArch.Application.Products;

namespace CleanArch.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Product").WithTags(nameof(ProductDto));

        group.MapGet("/", async (IProductService productService) =>
        {
            return Results.Ok(await productService.GetAllProducts());
        })
        .WithName("GetAllProducts")
        .WithOpenApi();

        group.MapGet("/{id}", async (int id, IProductService productService) =>
        {
            return Results.Ok(await productService.GetProductById(id));
        })
        .WithName("GetProductById")
        .WithOpenApi();

        group.MapPut("/{id}", async (int id, ProductDto productDto, IProductService productService) =>
        {
            return Results.Ok(await productService.UpdateProduct(id, productDto));
        })
        .WithName("UpdateProduct")
        .WithOpenApi();

        group.MapPost("/", async (ProductDto productDto, IProductService productService) =>
        {
            return Results.Ok(await productService.CreateProduct(productDto));
        })
        .WithName("CreateProduct")
        .WithValidation<ProductDto>()
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id, IProductService productService) =>
        {
            return Results.Ok(await productService.DeleteProduct(id));
        })
        .WithName("DeleteProduct")
        .WithOpenApi();
    }
}
