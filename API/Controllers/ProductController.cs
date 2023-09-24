using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Shared.Models;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductController : ControllerBase {
    private readonly DataContext _context;

    public ProductController(DataContext context) {
        _context = context;
    }

    // GET: api/Product
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts() {
        if (_context.Products == null) {
            return NotFound();
        }

        return await _context.Products.ToListAsync();
    }

    // GET: api/Product/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id) {
        if (_context.Products == null) {
            return NotFound();
        }

        var product = await _context.Products.FindAsync(id);

        if (product == null) {
            return NotFound();
        }

        return product;
    }

    // PUT: api/Product/5
    [Authorize("Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, Product product) {
        if (id != product.Id) {
            return BadRequest();
        }

        _context.Entry(product).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) {
            if (!ProductExists(id)) {
                return NotFound();
            }
            else {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Product
    [Authorize("Admin")]
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product) {
        if (_context.Products == null) {
            return Problem("Entity set 'DataContext.Products' is null.");
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    // DELETE: api/Product/5
    [Authorize("Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id) {
        if (_context.Products == null) {
            return NotFound();
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null) {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ProductExists(int id) {
        return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}