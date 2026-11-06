using CLApi.Data;
using CLApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    // 1. GET: /api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
     [FromQuery] string? search,
     [FromQuery] double? minPrice,
     [FromQuery] double? maxPrice)
    {
        var query = _context.Products.Where(x=>x.IsDeleted==false).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchTerm) ||
                                     p.Description.ToLower().Contains(searchTerm));
        }

        if (minPrice.HasValue && minPrice.Value > 0)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue && maxPrice.Value > 0)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        return await query.ToListAsync();
    }

    // 2. GET: /api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }

    // 3. POST: /api/products
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        const int maxRetries = 5;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            product.Id = RandomNumberGenerator.GetInt32(100000, 1000000);
            _context.Products.Add(product);

            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (DbUpdateException)
            {
                _context.Entry(product).State = EntityState.Detached;
                if (attempt == maxRetries - 1)
                {
                    return StatusCode(500, "Unable to generate a unique 6-digit product ID.");
                }
            }
        }

        return StatusCode(500, "Unexpected error during product creation.");
    }

    // 4. PUT: /api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, Product product)
    {
        if (id != product.Id)
        {
            return BadRequest("Product ID mismatch.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // 5. DELETE: /api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.IsDeleted = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // 6. POST: /api/products/{id}/decrement-stock/{quantity}
    [HttpPost("{id}/decrement-stock/{quantity}")]
    public async Task<IActionResult> DecrementStock(int id, int quantity)
    {
        if (quantity <= 0)
        {
            return BadRequest("Quantity must be greater than zero.");
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        if (product.Stock < quantity)
        {
            return BadRequest("Insufficient stock available.");
        }

        product.Stock -= quantity;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Stock decremented successfully", currentStock = product.Stock });
    }

    // 7. POST: /api/products/{id}/add-to-stock/{quantity}
    [HttpPost("{id}/add-to-stock/{quantity}")]
    public async Task<IActionResult> AddToStock(int id, int quantity)
    {
        if (quantity <= 0)
        {
            return BadRequest("Quantity must be greater than zero.");
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.Stock += quantity;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Stock incremented successfully", currentStock = product.Stock });
    }

    // 8. GET: /api/products/search?name={name}
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Product>>> SearchProducts([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Search name parameter is required.");
        }

        var products = await _context.Products
            .Where(p => p.Name.Contains(name))
            .ToListAsync();

        return products;
    }

    // 9. GET: /api/products/stock-level?min={min}&max={max}
    [HttpGet("stock-level")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByStockLevel([FromQuery] int min, [FromQuery] int max)
    {
        if (min > max)
        {
            return BadRequest("Minimum stock cannot exceed maximum stock.");
        }

        var products = await _context.Products
            .Where(p => p.Stock >= min && p.Stock <= max)
            .ToListAsync();

        return products;
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}