using InventoryMangmentSystem.Services;
using InventoryMangmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using InventoryMangmentSystem.Repositories;
namespace InventoryMangmentSystem.Controllers;

[ApiController]
[Route("api/[Controller]")]

public class CategoryController: ControllerBase
{
    private readonly GCategoryService _service;
    public CategoryController(GCategoryService service) 
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        var record = await _service.GetAll();
        return record.Any() ? Ok(record) : NotFound("No categories found");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetById(int id)
    {
        var record = await _service.GetByID(id);
        return record is null ? Ok(record) : NotFound("Category not found");
    }
    [HttpPost]
    public async Task<ActionResult<Category>> Create(Category category)
    {
        var record = await _service.Create(category);
        return CreatedAtAction(
            nameof(GetById),
            new {id =record.Id},
            record
        );
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Category category)
    {
        if (id != category.Id)
            throw new Exception("Miss match");
        await _service.Update(category);
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return NoContent();
    }
}
