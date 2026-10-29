using InventoryManagementSystem.Domain.Exceptions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Application.DTOs;
using InventoryManagementSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace InventoryManagementSystem.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class SaleController : ControllerBase
{
    private readonly SaleService _service;
    public SaleController(SaleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sale>>> GetAllAsync()
    {
        var sales = await _service.GetAllAsync();
        return Ok(sales);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sale>> GetByIdAsync(Guid id)
    {
        var sale = await _service.GetByIdAsync(id);
        return Ok(sale);
    }

    [HttpPost]
    public async Task<ActionResult<Sale>> Create([FromBody] SaleDto dto)
    {
        if (dto.Items is null || dto.Items.Count ==0)
            throw new BadRequestException(
                "A sale must contain at least one item.");
        var _sale = await _service.Create(dto);
        return CreatedAtAction(
            nameof(GetByIdAsync),
            new {id = _sale.Id},
            dto
        );
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id ,Sale sale)
    {
        if (id != sale.Id)
            throw new BadRequestException(
                "Route id does not match the sale id.");
        var _sale = await GetByIdAsync(id);
        await _service.Update(sale);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}