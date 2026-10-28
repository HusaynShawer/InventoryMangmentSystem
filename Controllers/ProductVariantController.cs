using InventoryManagementSystem.DTOs;
using InventoryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers;

[ApiController]
[Route("api/products/{productId}/variants")]
[Authorize]
public class ProductVariantController : ControllerBase
{
    private readonly ProductVariantService _service;

    public ProductVariantController(ProductVariantService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductVariantResponseDto>>> GetByProductId(Guid productId)
    {
        var variants = await _service.GetByProductId(productId);
        return Ok(variants);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductVariantResponseDto>> GetById(Guid productId, Guid id)
    {
        var variant = await _service.GetById(id);
        return Ok(variant);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ProductVariantResponseDto>> Create(Guid productId, [FromBody] CreateProductVariantDto dto)
    {
        var created = await _service.Create(productId, dto);
        return CreatedAtAction(nameof(GetById), new { productId, id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<ProductVariantResponseDto>> Update(Guid productId, Guid id, [FromBody] CreateProductVariantDto dto)
    {
        var updated = await _service.Update(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid productId, Guid id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}