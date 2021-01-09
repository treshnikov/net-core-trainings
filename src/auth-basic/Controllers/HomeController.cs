using Microsoft.AspNetCore.Mvc;

namespace auth_basic
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}