using CleanArch.Domain.Abstractions;

namespace CleanArch.Domain.Shared;

public interface ILogRepository : IGenericRepository<Log>
{
    Task<List<Log>> GetLogsByDates(DateTime startDate, DateTime endDate);
    Task<List<Log>> GetLogByApplicationAndDate(int applicationId, DateTime logged);
    Task<List<Log>> GetLogByApplicationAndYearAndMonth(int applicationId, DateTime logged);
    Task<List<Log>> GetLogByYearAndMonth(DateTime logged);
}
