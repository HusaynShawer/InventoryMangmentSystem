using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;


[ApiController]
[Route("/api[controller]")]
public class PurchaseItemsController : ControllerBase
{
    private readonly IPurchaseItemsService _service;
    public PurchaseItemsController(IPurchaseItemsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseItems>>> GetAll()
    {
        var _purchaseItems = await _service.GetAllAsync();
        return _purchaseItems.Any() ? Ok(_purchaseItems) : NotFound("No PurchaseItems found to return");
    }

    [HttpGet("id")]
    public async Task<ActionResult<PurchaseItems>> GetById(int id)
    {
        var _purchaseItem = await _service.GetByIdAsync(id);
        return _purchaseItem is null ? Ok(_purchaseItem) : NotFound("PurchaseItem not found to return");
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseItems>> Create(PurchaseItems purchaseItems)
    {
        var _purcahseItem = await _service.AddAsync(purchaseItems);
        return CreatedAtAction(
            nameof(GetById),
            new {id = _purcahseItem.Id},
            purchaseItems
        );
    }
    
    [HttpPut("id")]
    public async Task<ActionResult> Update(int id ,PurchaseItems purchaseItems)
    {
        if (id != purchaseItems.Id)
            throw new Exception("Miss match cant update purchaseItem");
        var _purchaseItem = await GetById(id);
        await _service.UpdateAsync(purchaseItems);
        return NoContent();
    }

    [HttpDelete("id")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}