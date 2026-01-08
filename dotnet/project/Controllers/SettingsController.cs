using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
