using CleanArch.API.Extensions;
using CleanArch.Application.Users;

namespace CleanArch.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/User").WithTags(nameof(UserDto));

        group.MapGet("/", async (IUserService userService) =>
        {
            return Results.Ok(await userService.GetAllUsers());
        })
        .WithName("GetAllUsers")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapGet("/{id}", async (int id, IUserService userService) =>
        {
            return Results.Ok(await userService.GetUserById(id));
        })
        .WithName("GetUserById")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPut("/{id}", async (int id, UserDto userDto, IUserService userService) =>
        {
            return Results.Ok(await userService.UpdateUser(id, userDto));
        })
        .WithName("UpdateUser")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPost("/", async (UserDto userDto, IUserService userService) =>
        {
            return Results.Ok(await userService.CreateUser(userDto));
        })
        .WithName("CreateUser")
        .WithFluentValidation<UserDto>()
        .WithOpenApi()
        .RequireAuthorization();

        group.MapDelete("/{id}", async (int id, IUserService userService) =>
        {
            return Results.Ok(await userService.DeleteUser(id));
        })
        .WithName("DeleteUser")
        .WithOpenApi()
        .RequireAuthorization();

        group.MapPost("/login", async (string username, string password, IUserService userService) =>
        {
            return Results.Ok(await userService.AuthenticateUser(username, password));
        })
        .WithName("LoginUser")
        .WithOpenApi();
    }
}
