using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierController : ControllerBase
{

    private readonly GSupplierService _service;


    public SupplierController(GSupplierService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Supplier>>> GetAll()
    {
        var _suppliers = await _service.GetAll();

        return _suppliers.Any() ? Ok(_suppliers) : NotFound("No Suppliers found");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Supplier>> GetById(Guid id)
    {
        var _supplier = await _service.GetByID(id);
        return _supplier is not null ? Ok(_supplier) : NotFound("Suppliers Not Found");

    }

    [HttpPost]
    public async Task<ActionResult<Supplier>> Create(Supplier supplier)
    {
        var _supplier = await _service.Create(supplier);

        return CreatedAtAction(
            nameof(GetById),
            new { id = _supplier.Id},
            _supplier
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Supplier supplier)
    {
        if (id != supplier.Id)
            return BadRequest("ID mismatch.");

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