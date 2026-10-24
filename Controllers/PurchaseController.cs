using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.DTOs;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace InventoryManagementSystem.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Policy ="AdminOnly")]
public class PurchaseController : ControllerBase
{
    private readonly PurchaseService _service;
    public PurchaseController(PurchaseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Purchase>>> GetAllAsync()
    {
        var purcahses = await _service.GetAllAsync();
        return Ok(purcahses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Purchase>> GetByIdAsync(Guid id)
    {
        var purcahse = await _service.GetByIdAsync(id);
        return Ok(purcahse);
    }

    [HttpPost]
    public async Task<ActionResult<Purchase>> Create([FromBody] PurchaseCreateDto dto)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            throw new BadRequestException(
                "A purchase must contain at least one item.");

        var Created = await _service.Create(dto);
        return Ok(dto);

    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id ,Purchase purchase)
    {
        if (id != purchase.Id)
            throw new BadRequestException(
                "Route id does not match the purchase id.");
        var purcahse = await GetByIdAsync(id);
        await _service.Update(purchase);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var purchase = await GetByIdAsync(id);
        await _service.Delete(id);
        return NoContent();
    }
}