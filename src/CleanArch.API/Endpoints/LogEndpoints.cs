using CleanArch.Application.Shared;

namespace CleanArch.API.Endpoints;

public static class LogEndpoints
{
    public static void MapLogEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Log").WithTags(nameof(LogDto));

        group.MapPost("/register", async (LogDto logDto, ILogService logService) =>
        {
            return Results.Ok(await logService.RegisterLog(logDto));
        })
        .WithName("RegisterLog")
        .WithOpenApi();

        group.MapGet("/dates", async (DateTime startDate, DateTime endDate, ILogService logService) =>
        {
            return Results.Ok(await logService.GetLogsByDates(startDate, endDate));
        })
        .WithName("GetLogsByDates")
        .WithOpenApi();

        group.MapGet("/appdate", async (int applicationId, DateTime logged, ILogService logService) =>
        {
            return Results.Ok(await logService.GetLogByApplicationAndDate(applicationId, logged));
        })
        .WithName("GetLogByApplicationAndDate")
        .WithOpenApi();

        group.MapGet("/appyearmonth", async (int applicationId, DateTime logged, ILogService logService) =>
        {
            return Results.Ok(await logService.GetLogByApplicationAndYearAndMonth(applicationId, logged));
        })
        .WithName("GetLogByApplicationAndYearAndMonth")
        .WithOpenApi();

        group.MapGet("/yearmonth", async (DateTime logged, ILogService logService) =>
        {
            return Results.Ok(await logService.GetLogByYearAndMonth(logged));
        })
        .WithName("GetLogByYearAndMonth")
        .WithOpenApi();
    }
}
