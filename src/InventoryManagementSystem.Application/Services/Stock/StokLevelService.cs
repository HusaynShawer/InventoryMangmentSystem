using InventoryManagementSystem.Application.Abstractions.Persistence;
using InventoryManagementSystem.Domain.Entities;


public class StokLevelService
{
    private readonly IStockLevelRepository _stockLevelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StokLevelService(
        IStockLevelRepository stockLevelRepository,
        IUnitOfWork unitOfWork)
    {
        _stockLevelRepository = stockLevelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<StockLevel> GetStockLevelAsync(
        Guid productVariantId, Guid warehouseId)
    {
        return await _stockLevelRepository.GetAsync(productVariantId, warehouseId);
    }

    public async Task<StockLevel> CreateStockLevelAsync(
        Guid productVariantId, Guid warehouseId)
    {
        var stockLevel = await _stockLevelRepository.GetOrCreateAsync(
            productVariantId, warehouseId);
        await _unitOfWork.SaveAsync();
        return stockLevel;
    }
}
