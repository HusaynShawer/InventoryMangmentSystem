using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryManagementSystem.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class SaleItemController : ControllerBase
{
    private readonly SaleItemService _service;
    public SaleItemController(SaleItemService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SaleItem>>> GetAllAsync()
    {
        var _salesItems = await _service.GetAllAsync();
        return Ok(_salesItems);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SaleItem>> GetByIdAsync(Guid id)
    {
        var _salesItem = await _service.GetByIdAsync(id);
        return Ok(_salesItem);
    }

    [HttpPost]
    public async Task<ActionResult<SaleItem>> Create(SaleItem saleItem)
    {
        var _saleItem = await _service.Create(saleItem);
        return CreatedAtAction(
            nameof(GetByIdAsync),
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
        var _salesItem = await GetByIdAsync(id);
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