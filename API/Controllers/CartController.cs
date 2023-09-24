using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Shared.Models;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController : ControllerBase {
    private readonly DataContext _context;

    public CartController(DataContext context) {
        _context = context;
    }

    // GET: api/Cart
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartPosition>>> GetCartPositions() {
        if (_context.CartPositions == null) {
            return NotFound();
        }

        if (User.IsInRole("Admin")) return await _context.CartPositions.ToListAsync();

        return await _context.CartPositions.Where(cp => IsUserAccessToCartPositionAllowed(User, cp)).ToListAsync();
    }

    // GET: api/Cart/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CartPosition>> GetCartPosition(int id) {
        if (_context.CartPositions == null) {
            return NotFound();
        }

        var cartPosition = await _context.CartPositions.FindAsync(id);

        if (cartPosition == null) {
            return NotFound();
        }

        if (!IsUserAccessToCartPositionAllowed(User, cartPosition)) {
            return Problem("You do not have access to the data.");
        }

        return cartPosition;
    }

    // PUT: api/Cart/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCartPosition(int id, CartPosition cartPosition) {
        if (!IsUserAccessToCartPositionAllowed(User, cartPosition)) {
            return Problem("You do not have access to the data.");
        }

        if (id != cartPosition.Id) {
            return BadRequest();
        }

        _context.Entry(cartPosition).State = EntityState.Modified;

        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) {
            if (!CartPositionExists(id)) {
                return NotFound();
            }
            else {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Cart
    [HttpPost]
    public async Task<ActionResult<CartPosition>> PostCartPosition(CartPosition cartPosition) {
        if (_context.CartPositions == null) {
            return Problem("Entity set 'DataContext.CartPositions'  is null.");
        }

        if (!IsUserAccessToCartPositionAllowed(User, cartPosition)) {
            return Problem("You do not have access to the data.");
        }

        _context.CartPositions.Add(cartPosition);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCartPosition", new { id = cartPosition.Id }, cartPosition);
    }

    // DELETE: api/Cart/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCartPosition(int id) {
        if (_context.CartPositions == null) {
            return NotFound();
        }

        var cartPosition = await _context.CartPositions.FindAsync(id);

        if (cartPosition == null) {
            return NotFound();
        }

        if (!IsUserAccessToCartPositionAllowed(User, cartPosition)) {
            return Problem("You do not have access to the data.");
        }

        _context.CartPositions.Remove(cartPosition);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CartPositionExists(int id) {
        return (_context.CartPositions?.Any(e => e.Id == id)).GetValueOrDefault();
    }

    private bool IsUserAccessToCartPositionAllowed(ClaimsPrincipal user, CartPosition cartPosition) {
        if (User.IsInRole("Admin")) return true;
        var userId = int.Parse(User.FindFirst("UserId")!.ToString());

        return cartPosition.UserId == userId;
    }
}