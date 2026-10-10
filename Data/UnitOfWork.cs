using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryMangmentSystem.Data;

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
}