using InventoryMangmentSystem.Exceptions;
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
        return Ok(_salesItems);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SaleItem>> GetById(Guid id)
    {
        var _salesItem = await _service.GetByID(id);
        return Ok(_salesItem);
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
    public async Task<ActionResult> Update(Guid id ,SaleItem saleItem)
    {
        if (id != saleItem.Id)
            throw new BadRequestException(
                "Route id does not match the sale item id.");
        var _salesItem = await GetById(id);
        await _service.Update(saleItem);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}