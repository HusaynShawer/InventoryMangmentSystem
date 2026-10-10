using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace InventoryMangmentSystem.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Policy ="AdminOnly")]
public class WarehouseController : ControllerBase
{
    private readonly GWarehouseService _service;
    public WarehouseController(GWarehouseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WarehouseResponseDTO>>> GetAll()
    {
        var warehouses = await _service.GetAll();

        return Ok(warehouses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WarehouseResponseDTO>> GetById(Guid id)
    {
        var warehouse = await _service.GetByID(id);

        return Ok(warehouse);
    }
    [HttpPost]
    public async Task<ActionResult<WarehouseResponseDTO>> Create(WarehouseDTO warehouseDTO)
    {
        var _supplier = await _service.Create(warehouseDTO);

        return CreatedAtAction(
            nameof(GetById),
            new { id = _supplier.Id},
            _supplier
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, WarehouseDTO warehouseDTO)
    {
        await _service.Update(id, warehouseDTO);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.Delete(id);
        return NoContent();
    }

    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<Product>>> GetwarehouseProducts()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var records = await _service.GetWarehouseProducts(userId);
        return Ok(records);
    }

    [HttpGet("products/low")]
    public async Task<ActionResult<IEnumerable<Product>>> GetWarehouseLowProducts()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var records = await _service.GetWarehouseLowProducts(userId);
        return Ok(records);
    }

}