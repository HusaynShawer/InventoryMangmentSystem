using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
namespace InventoryMangmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // حقن الـ DbContext
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/products (هنجيب كل المنتجات)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // POST: api/products (هضيف منتج جديد)
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
        }
    }
}