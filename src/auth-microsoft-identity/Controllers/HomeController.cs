using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auth_microsoft_identity
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

            return View();
        }
    }
}