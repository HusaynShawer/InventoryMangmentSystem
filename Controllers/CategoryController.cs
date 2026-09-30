using InventoryMangmentSystem.Services;
using InventoryMangmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
namespace InventoryMangmentSystem.Controllers;

[ApiController]
[Route("api/[Controller]")]

public class CategoryController: ControllerBase
{
    private readonly ICategoryService _service;
    public CategoryController(ICategoryService service) 
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        var record = await _service.GetAllAsync();
        return record.Any() ? Ok(record) : NotFound("No categories found");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetById(int id)
    {
        var record = await _service.GetByIdAsync(id);
        return record is null ? Ok(record) : NotFound("Category not found");
    }
    [HttpPost]
    public async Task<ActionResult<Category>> Create(Category category)
    {
        var record = await _service.AddAsync(category);
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
        await _service.UpdateAsync(category);
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
