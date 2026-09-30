using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{

    private readonly WarehouseService _service;


    public WarehouseController(WarehouseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Warehouse>>> GetAll()
    {
        var _warehouses = await _service.GetAllAsync();

        return _warehouses.Any() ? Ok(_warehouses) : NotFound("No Warehouse found");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Warehouse>> GetById(int id)
    {
        var _warehouse = await _service.GetByIdAsync(id);
        return _warehouse is not null ? Ok(_warehouse) : NotFound("Warehouse Not Found");

    }

    [HttpPost]
    public async Task<ActionResult<Warehouse>> Create(Warehouse warehouse)
    {
        var _warehouse = await _service.AddAsync(warehouse);

        return CreatedAtAction(
            nameof(GetById),
            new { id = _warehouse.Id},
            _warehouse
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Warehouse warehouse)
    {
        if (id != warehouse.Id)
            return BadRequest("ID mismatch.");

        await _service.UpdateAsync(warehouse);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}