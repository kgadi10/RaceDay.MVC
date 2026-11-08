using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers;

public class ParticipantController : Controller
{
    private readonly ApiService _api;

    public ParticipantController(ApiService api)
    {
        _api = api;
    }

    private bool IsParticipant()
    {
        return HttpContext.Session.GetString("UserRole") == "Participant"
            && !string.IsNullOrEmpty(
                HttpContext.Session.GetString("ApiCookie"));
    }

    public async Task<IActionResult> Dashboard()
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        var response = await _api.Get("api/Events");
        ViewBag.Events = await response.Content.ReadAsStringAsync();

        return View();
    }

    public async Task<IActionResult> Events()
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        var response = await _api.Get("api/Events");

        ViewBag.Data = await response.Content.ReadAsStringAsync();

        return View();
    }

    public async Task<IActionResult> Enrol(int id)
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        ViewBag.EventId = id;

        var response = await _api.Get($"api/Events/{id}/categories");
        ViewBag.Categories = await response.Content.ReadAsStringAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enrol(int eventId, int categoryId)
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        var response = await _api.Post("api/Enrolments", new
        {
            eventId,
            categoryId
        });

        TempData["Message"] = response.IsSuccessStatusCode
            ? "You have successfully entered the event."
            : "Your enrolment could not be completed.";

        return RedirectToAction("Events");
    }

    public async Task<IActionResult> Enrolments()
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        var response = await _api.Get("api/Enrolments/my");

        ViewBag.Data = await response.Content.ReadAsStringAsync();

        return View();
    }

    public async Task<IActionResult> Results()
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Account");

        var response = await _api.Get("api/Results/my");

        ViewBag.Data = await response.Content.ReadAsStringAsync();

        return View();
    }
}