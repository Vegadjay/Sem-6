using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class PagesController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Faq()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult Blank()
        {
            return View();
        }
    }
}
