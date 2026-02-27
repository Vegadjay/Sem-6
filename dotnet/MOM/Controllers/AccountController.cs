using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Mock Login - Any username/password works for demo
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}
