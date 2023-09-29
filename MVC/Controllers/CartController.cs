using Microsoft.AspNetCore.Mvc;
using MVC.Repository.Cart;
using Shared.Models;

namespace MVC.Controllers;

public class CartController : Controller {
    private readonly ICartRepository _cartRepository;

    public CartController(ICartRepository cartRepository) {
        _cartRepository = cartRepository;
    }

    // GET: CartView
    public async Task<IActionResult> Index() {
        var cart = await _cartRepository.GetCart(Request.Cookies["JwtToken"]!);
        return cart != null
            ? View(cart.ToList())
            : User.Identity!.IsAuthenticated
                ? Problem("Entity set 'Cart' is null.")
                : RedirectToAction("Login", "Account");
    }

    // GET: CartView/Details/5
    public async Task<IActionResult> Details(int? id) {
        if (id == null) {
            return NotFound();
        }

        var cartPosition = await _cartRepository.GetCartPosition((int)id, Request.Cookies["JwtToken"]!);

        if (cartPosition == null) {
            return NotFound();
        }

        return View(cartPosition);
    }

    // GET: CartView/Create
    public IActionResult Create() {
        // ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id"); 
        return View();
    }

    // POST: CartView/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,UserId,ProductId,Amount")] CartPosition cartPosition) {
        if (ModelState.IsValid) {
            await _cartRepository.AddCartPosition(cartPosition, Request.Cookies["JwtToken"]!);
            return RedirectToAction(nameof(Index));
        }
        // ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cartPosition.UserId);

        return View(cartPosition);
    }

    // GET: CartView/Edit/5
    public async Task<IActionResult> Edit(int? id) {
        if (id == null) {
            return NotFound();
        }

        var cartPosition = await _cartRepository.GetCartPosition((int)id, Request.Cookies["JwtToken"]!);

        if (cartPosition == null) {
            return NotFound();
        }
        // ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cartPosition.UserId);

        return View(cartPosition);
    }

    // POST: CartView/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ProductId,Amount")] CartPosition cartPosition) {
        if (id != cartPosition.Id) {
            return NotFound();
        }

        if (ModelState.IsValid) {
            try {
                await _cartRepository.UpdateCartPosition(id, cartPosition, Request.Cookies["JwtToken"]!);
            }
            catch (Exception) {
                if (!await CartPositionExists(cartPosition.Id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }
        // ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cartPosition.UserId);

        return View(cartPosition);
    }

    // GET: CartView/Delete/5
    public async Task<IActionResult> Delete(int? id) {
        if (id == null) {
            return NotFound();
        }

        var cartPosition = await _cartRepository.GetCartPosition((int)id, Request.Cookies["JwtToken"]!);

        if (cartPosition == null) {
            return NotFound();
        }

        return View(cartPosition);
    }

    // POST: CartView/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) {
        var product = await _cartRepository.GetCartPosition(id, Request.Cookies["JwtToken"]!);

        if (product != null) {
            await _cartRepository.RemoveCartPosition(id, Request.Cookies["JwtToken"]!);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> CartPositionExists(int id) {
        return await _cartRepository.GetCartPosition(id, Request.Cookies["JwtToken"]!) != null;
    }
}