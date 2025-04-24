using FluentValidation;
using FluentValidation.Results;

namespace CleanArch.API.Extensions;

public static class EndpointFluentValidationExtensions
{
    public static RouteHandlerBuilder WithFluentValidation<T>(this RouteHandlerBuilder builder) where T : class
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
            var model = context.Arguments.OfType<T>().FirstOrDefault();

            if (validator != null && model != null)
            {
                ValidationResult result = await validator.ValidateAsync(model);

                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(e => e.ErrorMessage).ToList();

                    return Results.BadRequest(new
                    {
                        IsSuccess = false,
                        Message = "Errores de validación.",
                        Data = errors,
                    });
                }
            }

            return await next(context);
        });
    }
}
