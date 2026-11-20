using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSinavPortali.Models;
using OnlineSinavPortali.Repositories;
using OnlineSinavPortali.ViewModels;

namespace OnlineSinavPortali.Controllers;

public sealed class AccountController(GenericRepository<User> users, IPasswordHasher<User> hasher) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        return User.IsInRole(Models.User.AdminRole)
            ? RedirectToAction("Index", "Admin")
            : View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userName = model.UserName.Trim();
        var user = await users.Query().SingleOrDefaultAsync(x => x.UserName == userName);
        if (user is null || user.Role != Models.User.AdminRole ||
            hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Kullanıcı adı veya parola hatalı.");
            return View(model);
        }

        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role)
        ], CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(new ClaimsPrincipal(identity));

        return Url.IsLocalUrl(model.ReturnUrl)
            ? LocalRedirect(model.ReturnUrl)
            : RedirectToAction("Index", "Admin");
    }

    [HttpPost]
    [Authorize(Roles = Models.User.AdminRole)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }

    [IgnoreAntiforgeryToken]
    public IActionResult Error() => View();
}
