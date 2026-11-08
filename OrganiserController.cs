using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers;

public class OrganiserController : Controller
{
    private readonly ApiService _api;

    public OrganiserController(ApiService api)
    {
        _api = api;
    }

    private bool IsOrganiser()
    {
        return HttpContext.Session.GetString("UserRole") == "Organiser"
            && !string.IsNullOrEmpty(
                HttpContext.Session.GetString("ApiCookie"));
    }

    public async Task<IActionResult> Dashboard()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        var response = await _api.Get("api/Events");

        ViewBag.Data = await response.Content.ReadAsStringAsync();

        return View();
    }

    public async Task<IActionResult> Events()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        var response = await _api.Get("api/Events");

        ViewBag.Data = await response.Content.ReadAsStringAsync();

        return View();
    }

    [HttpGet]
    public IActionResult CreateEvent()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(EventViewModel model)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        var response = await _api.Post("api/Events", new
        {
            name = model.Name,
            description = model.Description,
            date = model.Date,
            location = model.Location,
            distance = model.Distance,
            eventType = model.EventType
        });

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "The event could not be created.");
            return View(model);
        }

        TempData["Message"] = "Event created successfully.";

        return RedirectToAction("Events");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        var response = await _api.Delete($"api/Events/{id}");

        TempData["Message"] = response.IsSuccessStatusCode
            ? "Event deleted."
            : "The event could not be deleted.";

        return RedirectToAction("Events");
    }

    public async Task<IActionResult> Enrolments(int eventId)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        var response = await _api.Get(
            $"api/Events/{eventId}/enrolments");

        ViewBag.Data = await response.Content.ReadAsStringAsync();

        return View();
    }

    [HttpGet]
    public IActionResult AddResult()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddResult(ResultViewModel model)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        var response = await _api.Post("api/Results", new
        {
            enrolmentId = model.EnrolmentId,
            position = model.Position,
            finishTime = model.FinishTime
        });

        TempData["Message"] = response.IsSuccessStatusCode
            ? "Result captured successfully."
            : "The result could not be saved.";

        return RedirectToAction("Dashboard");
    }
}