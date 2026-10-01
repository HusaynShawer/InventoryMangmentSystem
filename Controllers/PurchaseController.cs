using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;


[ApiController]
[Route("/api[controller]")]
public class PurchaseController : ControllerBase
{
    private readonly GPurchaseService _service;
    public PurchaseController(GPurchaseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Purchase>>> GetAll()
    {
        var purcahses = await _service.GetAll();
        return purcahses.Any() ? Ok(purcahses) : NotFound("No purchases found to return");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Purchase>> GetById(int id)
    {
        var purcahse = await _service.GetByID(id);
        return purcahse is null ? Ok(purcahse) : NotFound("Putchase not found to return");
    }

    [HttpPost]
    public async Task<ActionResult<Purchase>> Create(Purchase purchase)
    {
        var _purchase = await _service.Create(purchase);
        return CreatedAtAction(
            nameof(GetById),
            new {id = _purchase.Id},
            purchase
        );
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id ,Purchase purchase)
    {
        if (id != purchase.Id)
            throw new Exception("Miss match cant update purchase");
        var purcahse = await GetById(id);
        await _service.Update(purchase);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var purchase = await GetById(id);
        await _service.Delete(id);
        return NoContent();
    }
}