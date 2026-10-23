using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;

namespace InventoryManagementSystem.Data;

public class UnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Save()
    {
        _context.SaveChanges();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Database.BeginTransactionAsync(
            cancellationToken);
    }

    public async Task CommitTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        await transaction.CommitAsync(cancellationToken);
        await transaction.DisposeAsync();
    }

    public async Task RollbackTransactionAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        await transaction.RollbackAsync(cancellationToken);
        await transaction.DisposeAsync();
    }


    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(
            _context,
            async (context, state, ct) =>
            {
                context.ChangeTracker.Clear();

                await using var transaction =
                    await context.Database.BeginTransactionAsync(ct);

                var result = await action();

                await transaction.CommitAsync(ct);

                return result;
            },
            verifySucceeded: null,
            cancellationToken: cancellationToken);
    }
}