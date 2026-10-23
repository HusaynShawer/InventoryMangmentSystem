using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierController : ControllerBase
{

    private readonly SupplierService _service;


    public SupplierController(SupplierService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetAllAsync()
    {
        var _suppliers = await _service.GetAllAsync();

        return Ok(_suppliers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Supplier>> GetByIdAsync(Guid id)
    {
        var _supplier = await _service.GetByIdAsync(id);
        return Ok(_supplier);

    }

    [HttpPost]
    public async Task<ActionResult<Supplier>> Create(Supplier supplier)
    {
        var _supplier = await _service.Create(supplier);

        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { id = _supplier.Id},
            _supplier
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Supplier supplier)
    {
        if (id != supplier.Id)
            throw new BadRequestException(
                "Route id does not match the supplier id.");

        await _service.Update(supplier);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.Delete(id);

        return NoContent();
    }
}