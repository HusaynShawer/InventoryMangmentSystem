using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Schemas;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Policy ="AdminOnly")]
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
    public async Task<ActionResult<Purchase>> GetById(Guid id)
    {
        var purcahse = await _service.GetByID(id);
        return purcahse is not null ? Ok(purcahse) : NotFound("Putchase not found to return");
    }

    [HttpPost]
    public async Task<ActionResult<Purchase>> Create([FromBody] PurchaseCreateDto dto)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            return BadRequest("A purchase must contain at least one item.");

        var Created = await _service.Create(dto);
        return Ok(dto);

    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id ,Purchase purchase)
    {
        if (id != purchase.Id)
            throw new Exception("Miss match cant update purchase");
        var purcahse = await GetById(id);
        await _service.Update(purchase);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var purchase = await GetById(id);
        await _service.Delete(id);
        return NoContent();
    }
}