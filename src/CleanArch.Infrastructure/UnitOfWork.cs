using System.Data;
using CleanArch.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArch.Infrastructure;

public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
{
    protected readonly TContext _context;

    public UnitOfWork(TContext context)
    {
        _context = context;
    }

    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}