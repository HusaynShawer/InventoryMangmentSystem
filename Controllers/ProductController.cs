using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{

    private readonly IProductService _service;


    public ProductController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _service.GetAllAsync();

        return products.Any() ? Ok(products) : NotFound("No Products found");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return product is not null ? Ok(product) : NotFound("Product Not Found");

    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        var createdProduct = await _service.AddAsync(product);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProduct.Id},
            createdProduct
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id)
            return BadRequest("ID mismatch.");

        await _service.UpdateAsync(product);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}