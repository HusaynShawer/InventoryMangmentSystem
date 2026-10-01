// using System.Diagnostics.Contracts;
// using InventoryMangmentSystem.Models;
// using InventoryMangmentSystem.Services;
// using Microsoft.AspNetCore.Mvc;
// namespace InventoryMangmentSystem.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class StockController : ControllerBase
// {

//     private readonly IStockService _service;


//     public StockController(IStockService service)
//     {
//         _service = service;
//     }

//     [HttpGet]
//     public async Task<ActionResult<IEnumerable<Stock>>> GetAll()
//     {
//         var _stocks = await _service.GetAllAsync();

//         return _stocks.Any() ? Ok(_stocks) : NotFound("No Stocks found");
//     }

//     [HttpGet("{id}")]
//     public async Task<ActionResult<Stock>> GetById(int id)
//     {
//         var _stock = await _service.GetByProductIdAsync(id);
//         return _stock is not null ? Ok(_stock) : NotFound("Stock Not Found");

//     }

//     [HttpPost]
//     public async Task<ActionResult<Stock>> Create(Stock stock)
//     {
//         var _stock = await _service.AddAsync(stock);

//         return CreatedAtAction(
//             nameof(GetById),
//             new { id = _stock.Id},
//             _stock
//         );
//     }

//     [HttpPut("{id}")]
//     public async Task<IActionResult> Update(int id, Stock stock)
//     {
//         if (id != stock.Id)
//             return BadRequest("ID mismatch.");

//         await _service.UpdateAsync(stock);

//         return NoContent();
//     }

//     [HttpDelete("{id}")]
//     public async Task<IActionResult> Delete(int id)
//     {
//         await _service.DeleteAsync(id);

//         return NoContent();
//     }

//     [HttpPatch("{id}/decrease")]
//     public async Task<ActionResult> DecreaseStockAsync(int id , int quantity)
//     {
//         await _service.DecreaseStockAsync(id,quantity);
//         return NoContent();
//     }
//     [HttpPatch("{id}/incarease")]
//     public async Task<ActionResult> IncreaseStockAsync(int id , int quantity)
//     {
//         await _service.IncreaseStockAsync(id,quantity);
//         return NoContent();
//     }
// }