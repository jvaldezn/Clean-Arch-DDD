using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace CleanArch.Domain.Abstractions;

public interface IUnitOfWork<TContext> : IAsyncDisposable where TContext : class
{
    Task<int> SaveAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}
