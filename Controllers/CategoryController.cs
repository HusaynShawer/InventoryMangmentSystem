using InventoryMangmentSystem.Services;
using InventoryMangmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using InventoryMangmentSystem.Repositories;
using InventoryMangmentSystem.Schemas;
using Microsoft.AspNetCore.Authorization;
namespace InventoryMangmentSystem.Controllers;

[ApiController]
[Route("api/[Controller]")]
[Authorize(Policy ="AdminOnly")]
public class CategoryController: ControllerBase
{
    private readonly GCategoryService _service;
    public CategoryController(GCategoryService service) 
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll()
    {
        var record = await _service.GetAll();
        return record.Any() ? Ok(record) : NotFound("No categories found");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseDTO>> GetById(Guid id)
    {
        var record = await _service.GetByID(id);
        return record is not null ? Ok(record) : NotFound("Category not found");
    }
    [HttpPost]
    public async Task<ActionResult<CategoryResponseDTO>> Create(CategoryDTO dTO)
    {
        var record = await _service.Create(dTO);
        return CreatedAtAction(
            nameof(GetById),
            new {id =record.Id},
            record
        );
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, CategoryDTO dTO)
    {

        await _service.Update(id,dTO);
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}
