using System.Security.Claims;
using InventoryMangmentSystem.Schemas;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMangmentSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly GProductService _service;

    public ProductController(GProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetAll()
    {
        var products = await _service.GetAll();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDTO>> GetById(int id)
    {
        try
        {
            var product = await _service.GetByID(id);
            return Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProductResponseDTO>> Create(ProductDTO dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var createdProduct = await _service.Create(userId, dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProduct.Id },
            createdProduct
        );
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<ProductResponseDTO>> Update(int id, ProductDTO dto)
    {
        try
        {
            var updated = await _service.Update(id, dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.Delete(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}