using CleanArch.Domain.Abstractions;

namespace CleanArch.Application.Shared;

public interface ILogService
{
    Task<Response<LogDto>> RegisterLog(LogDto information);
    Task<Response<IEnumerable<LogDto>>> GetLogsByDates(DateTime startDate, DateTime endDate);
    Task<Response<IEnumerable<LogDto>>> GetLogByApplicationAndDate(int applicationId, DateTime logged);
    Task<Response<IEnumerable<LogDto>>> GetLogByApplicationAndYearAndMonth(int applicationId, DateTime logged);
    Task<Response<IEnumerable<LogDto>>> GetLogByYearAndMonth(DateTime logged);
}
