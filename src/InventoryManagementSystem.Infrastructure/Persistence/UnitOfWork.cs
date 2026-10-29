using InventoryManagementSystem.Application.Abstractions.Persistence;

namespace InventoryManagementSystem.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
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
