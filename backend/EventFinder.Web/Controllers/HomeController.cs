using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EventFinder.Web.Models;
using EventFinder.Web.DTOs.Events;
using EventFinder.Web.Services;

namespace EventFinder.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEventService _eventService;

    public HomeController(ILogger<HomeController> logger, IEventService eventService)
    {
        _logger = logger;
        _eventService = eventService;
    }

    public async Task<IActionResult> Index()
    {
        var upcoming = await _eventService.GetAllAsync(new EventFilterDto());
        var recentEvents = upcoming.Take(8).ToList();
        return View(recentEvents);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
