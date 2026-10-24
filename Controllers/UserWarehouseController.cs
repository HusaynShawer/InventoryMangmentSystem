using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace InventoryManagementSystem.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class UserWarehouseController : ControllerBase
{
    private readonly UserWarehouseService _service;
    public UserWarehouseController(UserWarehouseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserWarehouse>>> GetAllAsync()
    {
        var warehouses = await _service.GetAllAsync();

        return Ok(warehouses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserWarehouse>> GetByIdAsync(Guid id)
    {
        var warehouse = await _service.GetByIdAsync(id);

        return Ok(warehouse);
    }
    [HttpPost]
    public async Task<ActionResult<UserWarehouse>> Create(UserWarehouse userWarehouse)
    {
        var _supplier = await _service.Create(userWarehouse);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = _supplier.Id},
            _supplier
        );
    }

    // [HttpPut("{id}")]
    // public async Task<IActionResult> Update(Guid id, Warehouse warehouse)
    // {
    //     if (id != warehouse.Id)
    //         return BadRequest("ID mismatch.");

    //     await _service.Update(warehouse);

    //     return NoContent();
    // }

    // [HttpDelete("{id}")]
    // public async Task<IActionResult> Delete(Guid id)
    // {
    //     await _service.Delete(id);
    //     return NoContent();
    // }
}