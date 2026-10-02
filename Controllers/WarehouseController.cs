using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace InventoryMangmentSystem.Controllers;


[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly GWarehouseService _service;
    public WarehouseController(GWarehouseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Warehouse>>> GetAll()
    {
        var warehouses = await _service.GetAll();

        return Ok(warehouses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Warehouse>> GetById(int id)
    {
        var warehouse = await _service.GetByID(id);

        return Ok(warehouse);
    }
    [HttpPost]
    public async Task<ActionResult<Warehouse>> Create(Warehouse warehouse)
    {
        var _supplier = await _service.Create(warehouse);

        return CreatedAtAction(
            nameof(GetById),
            new { id = _supplier.Id},
            _supplier
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Warehouse warehouse)
    {
        if (id != warehouse.Id)
            return BadRequest("ID mismatch.");

        await _service.Update(warehouse);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }

    [HttpGet("products")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Product>>> GetwarehouseProducts()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var records = await _service.GetWarehouseProducts(userId);
        return Ok(records);
    }

    [HttpGet("products/low")]
    public async Task<ActionResult<IEnumerable<Product>>> GetWarehouseLowProducts()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var records = await _service.GetWarehouseLowProducts(userId);
        return Ok(records);
    }

}