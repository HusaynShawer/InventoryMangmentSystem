using InventoryManagementSystem.DTOs;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers;

[ApiController]
[Route("api/product-attributes")]
[Authorize(Policy = "AdminOnly")]
public class ProductAttributeController : ControllerBase
{
    private readonly ProductAttributeService _service;

    public ProductAttributeController(ProductAttributeService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductAttributeResponseDto>>> GetAll()
    {
        var attributes = await _service.GetAllAsync();
        return Ok(attributes);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductAttributeResponseDto>> GetById(Guid id)
    {
        var attribute = await _service.GetByIdAsync(id);
        return Ok(attribute);
    }

    [HttpPost]
    public async Task<ActionResult<ProductAttributeResponseDto>> Create([FromBody] ProductAttributeDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductAttributeResponseDto>> Update(Guid id, [FromBody] ProductAttributeDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/values")]
    public async Task<ActionResult<ProductAttributeValueResponseDto>> AddValue(Guid id, [FromBody] ProductAttributeValueDto dto)
    {
        var value = await _service.AddValueAsync(id, dto);
        return Ok(value);
    }

    [HttpDelete("{id}/values/{valueId}")]
    public async Task<IActionResult> DeleteValue(Guid id, Guid valueId)
    {
        await _service.DeleteValueAsync(id, valueId);
        return NoContent();
    }
}
