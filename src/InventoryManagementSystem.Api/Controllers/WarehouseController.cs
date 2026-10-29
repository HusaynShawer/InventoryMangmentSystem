using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace InventoryManagementSystem.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Policy ="AdminOnly")]
public class WarehouseController : ControllerBase
{
    private readonly WarehouseService _service;
    public WarehouseController(WarehouseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WarehouseResponseDto>>> GetAllAsync()
    {
        var warehouses = await _service.GetAllAsync();

        return Ok(warehouses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WarehouseResponseDto>> GetByIdAsync(Guid id)
    {
        var warehouse = await _service.GetByIdAsync(id);

        return Ok(warehouse);
    }
    [HttpPost]
    public async Task<ActionResult<WarehouseResponseDto>> Create(WarehouseDto warehouseDTO)
    {

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        if (!Guid.TryParse(userIdClaim,out var userId))
            return Unauthorized();

        var _supplier = await _service.Create(userId, warehouseDTO);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = _supplier.Id},
            _supplier
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, WarehouseDto warehouseDTO)
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