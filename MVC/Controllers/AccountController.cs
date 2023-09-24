using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MVC.Utils;
using Shared.Models;

namespace MVC.Controllers;

public class AccountController : Controller {
    private readonly ITokenHelper _tokenHelper;

    public AccountController(ITokenHelper tokenHelper) {
        _tokenHelper = tokenHelper;
    }

    [HttpGet]
    public IActionResult Login() {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel loginModel) {
        if (ModelState.IsValid) {
            var authData = await _tokenHelper.GetToken(loginModel);
            if (authData != null) {
                await Authenticate(authData);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Incorrect login or(and) password.");
        }

        return View(loginModel);
    }

    [HttpGet]
    public IActionResult Register() {
        return View();
    }

    //
    // [HttpPost]
    // [ValidateAntiForgeryToken]
    // public async Task<IActionResult> Register(RegisterModel model) {
    //     if (ModelState.IsValid) {
    //         User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Login);
    //         if (user == null) {
    //             // добавляем пользователя в бд
    //             user = new User { Email = model.Login, Password = model.Password };
    //             Role userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
    //             if (userRole != null)
    //                 user.Role = userRole;
    //
    //             db.Users.Add(user);
    //             await db.SaveChangesAsync();
    //
    //             await Authenticate(user); // аутентификация
    //
    //             return RedirectToAction("Index", "Home");
    //         }
    //         else
    //             ModelState.AddModelError("", "Некорректные логин и(или) пароль");
    //     }
    //
    //     return View(model);
    // }

    private async Task Authenticate(TokenControllerPostResponse response) {
        Response.Cookies.Append("JwtToken", response.JwtToken!);
        var claims = new List<Claim> {
            new(ClaimsIdentity.DefaultNameClaimType, response.Login!),
        };
        if (response.Role != null) claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, response.Role!.Name!));
        
        var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }

    public async Task<IActionResult> Logout() {
        Response.Cookies.Delete("JwtToken");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}