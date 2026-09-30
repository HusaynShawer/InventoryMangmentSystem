using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;

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
    public async Task<ActionResult<IEnumerable<Supplier>>> GetAll()
    {
        var _suppliers = await _service.GetAllAsync();

        return _suppliers.Any() ? Ok(_suppliers) : NotFound("No Suppliers found");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Supplier>> GetById(int id)
    {
        var _supplier = await _service.GetByIdAsync(id);
        return _supplier is not null ? Ok(_supplier) : NotFound("Suppliers Not Found");

    }

    [HttpPost]
    public async Task<ActionResult<Supplier>> Create(Supplier supplier)
    {
        var _supplier = await _service.AddAsync(supplier);

        return CreatedAtAction(
            nameof(GetById),
            new { id = _supplier.Id},
            _supplier
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Supplier supplier)
    {
        if (id != supplier.Id)
            return BadRequest("ID mismatch.");

        await _service.UpdateAsync(supplier);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}