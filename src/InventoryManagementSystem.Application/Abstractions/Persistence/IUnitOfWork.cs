namespace InventoryManagementSystem.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task SaveAsync(CancellationToken cancellationToken = default);

    void Save();

    Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default);
}
