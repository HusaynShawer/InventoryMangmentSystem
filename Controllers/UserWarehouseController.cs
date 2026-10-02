using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace InventoryMangmentSystem.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UserWarehouseController : ControllerBase
{
    private readonly UserWarehouseService _service;
    public UserWarehouseController(UserWarehouseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserWarehouse>>> GetAll()
    {
        var warehouses = await _service.GetAll();

        return Ok(warehouses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserWarehouse>> GetById(int id)
    {
        var warehouse = await _service.GetByID(id);

        return Ok(warehouse);
    }
    [HttpPost]
    public async Task<ActionResult<UserWarehouse>> Create(UserWarehouse userWarehouse)
    {
        var _supplier = await _service.Create(userWarehouse);

        return CreatedAtAction(
            nameof(GetById),
            new { id = _supplier.Id},
            _supplier
        );
    }

    // [HttpPut("{id}")]
    // public async Task<IActionResult> Update(int id, Warehouse warehouse)
    // {
    //     if (id != warehouse.Id)
    //         return BadRequest("ID mismatch.");

    //     await _service.Update(warehouse);

    //     return NoContent();
    // }

    // [HttpDelete("{id}")]
    // public async Task<IActionResult> Delete(int id)
    // {
    //     await _service.Delete(id);
    //     return NoContent();
    // }
}