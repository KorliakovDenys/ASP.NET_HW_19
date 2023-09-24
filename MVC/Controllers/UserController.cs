using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Repository.User;
using Shared.Models;

namespace MVC.Controllers;

[Authorize]
public class UserController : Controller {
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository) {
        _userRepository = userRepository;
    }

    // GET: UserView
    public async Task<IActionResult> Index() {
        var users = await _userRepository.GetUsers(Request.Cookies["JwtToken"]!);
        return users != null
            ? View(users.ToList())
            : Problem("Entity set 'Products' is null.");
    }

    // GET: UserView/Details/5
    public async Task<IActionResult> Details(int? id) {
        if (id == null) {
            return NotFound();
        }

        var user = await _userRepository.GetUser((int)id, Request.Cookies["JwtToken"]!);

        if (user == null) {
            return NotFound();
        }

        return View(user);
    }

    // GET: UserView/Create
    public IActionResult Create() {
        return View();
    }

    // POST: UserView/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Login,Password")] User user) {
        if (ModelState.IsValid) {
            await _userRepository.AddUser(user, Request.Cookies["JwtToken"]!);
            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }

    // GET: UserView/Edit/5
    public async Task<IActionResult> Edit(int? id) {
        if (id == null) {
            return NotFound();
        }

        var product = await _userRepository.GetUser((int)id, Request.Cookies["JwtToken"]!);

        if (product == null) {
            return NotFound();
        }

        return View(product);
    }

    // POST: UserView/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Login,Password")] User user) {
        if (id != user.Id) {
            return NotFound();
        }

        if (ModelState.IsValid) {
            try {
                await _userRepository.UpdateUser(id, user, Request.Cookies["JwtToken"]!);
            }
            catch (Exception) {
                if (!await UserExists(user.Id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }

    // GET: UserView/Delete/5
    public async Task<IActionResult> Delete(int? id) {
        if (id == null) {
            return NotFound();
        }

        var product = await _userRepository.GetUser((int)id, Request.Cookies["JwtToken"]!);

        if (product == null) {
            return NotFound();
        }

        return View(product);
    }

    // POST: UserView/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) {
        var product = await _userRepository.GetUser(id, Request.Cookies["JwtToken"]!);

        if (product != null) {
            await _userRepository.RemoveUser(id, Request.Cookies["JwtToken"]!);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> UserExists(int id) {
        return await _userRepository.GetUser(id, Request.Cookies["JwtToken"]!) != null;
    }
}