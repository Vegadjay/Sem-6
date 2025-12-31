using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using razor_demo.Models;

namespace razor_demo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        TempData["greeting"] = "Hey there";
        return RedirectToAction("Privacy");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Final()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
