using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SaleItemController : ControllerBase
{
    private readonly GsaleItemService _service;
    public SaleItemController(GsaleItemService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SaleItem>>> GetAll()
    {
        var _salesItems = await _service.GetAll();
        return _salesItems.Any() ? Ok(_salesItems) : NotFound("No Sale Item found to return");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SaleItem>> GetById(int id)
    {
        var _salesItem = await _service.GetByID(id);
        return _salesItem is not null ? Ok(_salesItem) : NotFound("sale item not found to return");
    }

    [HttpPost]
    public async Task<ActionResult<SaleItem>> Create(SaleItem saleItem)
    {
        var _saleItem = await _service.Create(saleItem);
        return CreatedAtAction(
            nameof(GetById),
            new {id = _saleItem.Id},
            _saleItem
        );
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id ,SaleItem saleItem)
    {
        if (id != saleItem.Id)
            throw new Exception("Miss match cant update sale item");
        var _salesItem = await GetById(id);
        await _service.Update(saleItem);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}