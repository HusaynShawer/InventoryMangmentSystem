using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockLevelController : ControllerBase
{
    private readonly StokLevelService _stockLevelService;

    public StockLevelController(StokLevelService stockLevelService)
    {
        _stockLevelService = stockLevelService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStockLevel(
        Guid productVariantId, Guid warehouseId)
    {
        var stockLevel = await _stockLevelService.GetStockLevelAsync(
            productVariantId, warehouseId);
        return Ok(stockLevel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStockLevel(
        Guid productVariantId, Guid warehouseId)
    {
        var stockLevel = await _stockLevelService.CreateStockLevelAsync(
            productVariantId, warehouseId);
        return Ok(stockLevel);
    }
}
