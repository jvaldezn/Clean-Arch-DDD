using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace CleanArch.API.Middlewares;

public static class ExceptionHandlingMiddleware
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                var response = new
                {
                    IsSuccess = false,
                    Message = "Error del sistema, comuníquese con el administrador",
                    Data = exception?.Message + ": " + exception?.StackTrace,
                };

                var json = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(json);
            });
        });
    }
}
