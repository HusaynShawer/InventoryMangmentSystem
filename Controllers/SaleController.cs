using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;


[ApiController]
[Route("/api[controller]")]
public class SaleController : ControllerBase
{
    private readonly ISaleService _service;
    public SaleController(ISaleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sale>>> GetAll()
    {
        var sales = await _service.GetAllAsync();
        return sales.Any() ? Ok(sales) : NotFound("No sales found to return");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sale>> GetById(int id)
    {
        var sale = await _service.GetByIdAsync(id);
        return sale is null ? Ok(sale) : NotFound("sale not found to return");
    }

    [HttpPost]
    public async Task<ActionResult<Sale>> Create(Sale sale)
    {
        var _sale = await _service.AddAsync(sale);
        return CreatedAtAction(
            nameof(GetById),
            new {id = _sale.Id},
            _sale
        );
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id ,Sale sale)
    {
        if (id != sale.Id)
            throw new Exception("Miss match cant update sale");
        var _sale = await GetById(id);
        await _service.UpdateAsync(sale);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}