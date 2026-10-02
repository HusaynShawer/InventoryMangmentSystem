
namespace InventoryMangmentSystem.Data;

public class UnitOfWork 
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}