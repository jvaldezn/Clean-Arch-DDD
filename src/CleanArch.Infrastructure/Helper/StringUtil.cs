using FluentValidation.Results;

namespace CleanArch.Infrastructure.Helper;

public static class StringUtil
{
    public static string GetErrorMessage(this IEnumerable<ValidationFailure> errors) => string.Join(", ", errors);
}
