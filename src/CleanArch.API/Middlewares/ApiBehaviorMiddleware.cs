using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Middlewares;

public static class ApiBehaviorMiddleware
{
    public static IServiceCollection ConfigureCustomApiBehavior(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = false;

            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join(" ", errors);

                var response = new
                {
                    IsSuccess = false,
                    Message = errorMessage,
                    Data = errors,
                };

                return new BadRequestObjectResult(response);
            };
        });

        return services;
    }
}
