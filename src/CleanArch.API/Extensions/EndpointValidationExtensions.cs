using System.ComponentModel.DataAnnotations;

namespace CleanArch.API.Extensions;

public static class EndpointValidationExtensions
{
    public static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            var dto = context.Arguments.OfType<T>().FirstOrDefault();
            if (dto is not null)
            {
                var validationContext = new ValidationContext(dto);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(dto, validationContext, results, true))
                {
                    var errors = results.Select(e => e.ErrorMessage).ToList();
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
