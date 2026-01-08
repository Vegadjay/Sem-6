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

    public IActionResult RedirectToGitHub() {
        return Redirect("https://github.com/Vegadjay");
    }

    public IActionResult GetContent() {
        return Content("Something went wrong.");
    }

    public IActionResult GetFile() {
        return File("~/favicon.ico", "image/x-icon", "favicon.ico");
    }

    public IActionResult GetJson() {
        return Json(new {
            Name = "Jay Vegad",
            EnrollmentNumber = "23010101294",
            Semester = 1,
            SPI = 9.0f
        });
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
