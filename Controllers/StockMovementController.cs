using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockMovementController : ControllerBase
{

    private readonly StockMovementService _service;


    public StockMovementController(StockMovementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockMovement>>> GetAll()
    {
        var _stocksMovement = await _service.GetAllAsync();

        return _stocksMovement.Any() ? Ok(_stocksMovement) : NotFound("No Stock Movements found");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockMovement>> GetById(int id)
    {
        var _stockMovement = await _service.GetByIdAsync(id);
        return _stockMovement is not null ? Ok(_stockMovement) : NotFound("Stock movement Not Found");

    }

    [HttpPost]
    public async Task<ActionResult<Stock>> Create(StockMovement stockMovement)
    {
        var _stockMovement = await _service.AddAsync(stockMovement);

        return CreatedAtAction(
            nameof(GetById),
            new { id = _stockMovement.Id},
            _stockMovement
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, StockMovement stockMovement)
    {
        if (id != stockMovement.Id)
            return BadRequest("ID mismatch.");

        await _service.UpdateAsync(stockMovement);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}