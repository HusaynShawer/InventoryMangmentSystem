using InventoryManagementSystem.Application.Services;
using InventoryManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[Controller]")]
[Authorize(Policy ="AdminOnly")]
public class CategoryController: ControllerBase
{
    private readonly CategoryService _service;
    public CategoryController(CategoryService service) 
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllAsync()
    {
        var record = await _service.GetAllAsync();
        return Ok(record);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> GetByIdAsync(Guid id)
    {
        var record = await _service.GetByIdAsync(id);
        return Ok(record);
    }
    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> Create(CategoryDto dTO)
    {
        var record = await _service.Create(dTO);
        return CreatedAtAction(
            nameof(GetByIdAsync),
            new {id =record.Id},
            record
        );
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, CategoryDto dTO)
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
