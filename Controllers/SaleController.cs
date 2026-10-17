using InventoryMangmentSystem.Exceptions;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Schemas;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class SaleController : ControllerBase
{
    private readonly GsaleService _service;
    public SaleController(GsaleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sale>>> GetAll()
    {
        var sales = await _service.GetAll();
        return Ok(sales);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sale>> GetById(Guid id)
    {
        var sale = await _service.GetByID(id);
        return Ok(sale);
    }

    [HttpPost]
    public async Task<ActionResult<Sale>> Create([FromBody] SaleDTO dto)
    {
        if (dto.Items is null || dto.Items.Count ==0)
            throw new BadRequestException(
                "A sale must contain at least one item.");
        var _sale = await _service.Create(dto);
        return CreatedAtAction(
            nameof(GetById),
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
        var _sale = await GetById(id);
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