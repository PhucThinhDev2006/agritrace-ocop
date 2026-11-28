using AgriTrace.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
namespace AgriTrace.Infrastructure.Persistence.Repositories;

public class EfUnitOfWork(AgriTraceDbContext db) : IUnitOfWork
{
    private IDbContextTransaction? _currentTransaction;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is not null)
        {
            return;
        }
        _currentTransaction = await db.Database.BeginTransactionAsync(ct);
    }
    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is null)
        {
            return;
        }
        try
        {
            await db.SaveChangesAsync(ct);
            await _currentTransaction.CommitAsync(ct);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction is null)
        {
            return;
        }
        try
        {
            await _currentTransaction.RollbackAsync(ct);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }
}