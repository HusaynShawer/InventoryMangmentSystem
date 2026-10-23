using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryManagementSystem.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PurchaseItemsController : ControllerBase
{
    private readonly PurchaseItemService _service;
    public PurchaseItemsController(PurchaseItemService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseItem>>> GetAllAsync()
    {
        var _purchaseItems = await _service.GetAllAsync();
        return Ok(_purchaseItems);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseItem>> GetByIdAsync(Guid id)
    {
        var _purchaseItem = await _service.GetByIdAsync(id);
        return Ok(_purchaseItem);
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseItem>> Create(PurchaseItem purchaseItems)
    {
        var _purcahseItem = await _service.Create(purchaseItems);
        return CreatedAtAction(
            nameof(GetByIdAsync),
            new {id = _purcahseItem.Id},
            purchaseItems
        );
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id ,PurchaseItem purchaseItems)
    {
        if (id != purchaseItems.Id)
            throw new BadRequestException(
                "Route id does not match the purchase item id.");
        var _purchaseItem = await GetByIdAsync(id);
        await _service.Update(purchaseItems);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}