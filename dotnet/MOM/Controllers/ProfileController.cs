using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
