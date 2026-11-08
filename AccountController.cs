using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers;

public class AccountController : Controller
{
    private readonly ApiService _api;

    public AccountController(ApiService api)
    {
        _api = api;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await _api.Login(new
        {
            email = model.Email,
            password = model.Password
        });

        if (!success)
        {
            ModelState.AddModelError("", "Login failed. Check your details and API authentication response.");
            return View(model);
        }

        HttpContext.Session.SetString("UserEmail", model.Email);
        HttpContext.Session.SetString("UserRole", model.Role);

        return RedirectToAction(
            "Dashboard",
            model.Role == "Organiser" ? "Organiser" : "Participant");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await _api.Post("api/Auth/register", new
        {
            fullName = model.FullName,
            email = model.Email,
            password = model.Password,
            role = model.Role
        });

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Registration failed. Please check your details.");
            return View(model);
        }

        TempData["Message"] = "Registration successful. Please log in.";

        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        try
        {
            await _api.Post("api/Auth/logout", new { });
        }
        catch
        {
            // The local session is still cleared.
        }

        _api.Logout();

        return RedirectToAction("Index", "Home");
    }
}