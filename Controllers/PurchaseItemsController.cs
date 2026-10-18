using InventoryMangmentSystem.Exceptions;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PurchaseItemsController : ControllerBase
{
    private readonly GPurchaseItemsService _service;
    public PurchaseItemsController(GPurchaseItemsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseItems>>> GetAll()
    {
        var _purchaseItems = await _service.GetAll();
        return Ok(_purchaseItems);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseItems>> GetById(Guid id)
    {
        var _purchaseItem = await _service.GetByID(id);
        return Ok(_purchaseItem);
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseItems>> Create(PurchaseItems purchaseItems)
    {
        var _purcahseItem = await _service.Create(purchaseItems);
        return CreatedAtAction(
            nameof(GetById),
            new {id = _purcahseItem.Id},
            purchaseItems
        );
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id ,PurchaseItems purchaseItems)
    {
        if (id != purchaseItems.Id)
            throw new BadRequestException(
                "Route id does not match the purchase item id.");
        var _purchaseItem = await GetById(id);
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