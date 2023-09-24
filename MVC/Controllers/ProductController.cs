using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Repository.Product;
using Shared.Models;

namespace MVC.Controllers;

[Authorize]
public class ProductController : Controller {
    private readonly IProductRepository _productRepository;

    public ProductController(IProductRepository productRepository) {
        _productRepository = productRepository;
    }

    // GET: ProductView
    public async Task<IActionResult> Index() {
        var products = await _productRepository.GetProducts(Request.Cookies["JwtToken"]!);
        return products != null
            ? View(products.ToList())
            : User.Identity!.IsAuthenticated
                ? Problem("Entity set 'Product' is null.")
                : RedirectToAction("Login", "Account");
    }

    // GET: ProductView/Details/5
    public async Task<IActionResult> Details(int? id) {
        if (id == null) {
            return NotFound();
        }

        var product = await _productRepository.GetProduct((int)id, Request.Cookies["JwtToken"]!);

        if (product == null) {
            return NotFound();
        }

        return View(product);
    }

    // GET: ProductView/Create
    public IActionResult Create() {
        return View();
    }

    // POST: ProductView/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Price")] Product product) {
        if (ModelState.IsValid) {
            await _productRepository.AddProduct(product, Request.Cookies["JwtToken"]!);
            return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    // GET: ProductView/Edit/5
    public async Task<IActionResult> Edit(int? id) {
        if (id == null) {
            return NotFound();
        }

        var product = await _productRepository.GetProduct((int)id, Request.Cookies["JwtToken"]!);

        if (product == null) {
            return NotFound();
        }

        return View(product);
    }

    // POST: ProductView/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price")] Product product) {
        if (id != product.Id) {
            return NotFound();
        }

        if (ModelState.IsValid) {
            try {
                await _productRepository.UpdateProduct(id, product, Request.Cookies["JwtToken"]!);
            }
            catch (Exception) {
                if (!await ProductExists(product.Id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    // GET: ProductView/Delete/5
    public async Task<IActionResult> Delete(int? id) {
        if (id == null) {
            return NotFound();
        }

        var product = await _productRepository.GetProduct((int)id, Request.Cookies["JwtToken"]!);

        if (product == null) {
            return NotFound();
        }

        return View(product);
    }

    // POST: ProductView/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) {
        var product = await _productRepository.GetProduct(id, Request.Cookies["JwtToken"]!);

        if (product != null) {
            await _productRepository.RemoveProduct(id, Request.Cookies["JwtToken"]!);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> ProductExists(int id) {
        return await _productRepository.GetProduct(id, Request.Cookies["JwtToken"]!) != null;
    }
}