using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace auth_microsoft_identity
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AdminController(
            IJwtGenerator jwtGenerator,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            this._jwtGenerator = jwtGenerator;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            var signinResult = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (signinResult.Succeeded)
            {
                return Redirect(model.ReturnUrl);
            }

            model.Token = _jwtGenerator.CreateToken(user);
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<LoginViewModel>> Signin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return Ok(model);
            }

            var signinResult = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (signinResult.Succeeded)
            {
                model.Token = _jwtGenerator.CreateToken(user);
                return Ok(model);
            }
            
            return ValidationProblem();
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Home/Index");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        //[Authorize(Policy = "SuperUser")]
        [Authorize(Roles = "Administrator123")]
        public IActionResult Administrator()
        {
            return View();
        }
        [Authorize(Policy = "SuperUser")]
        public IActionResult Manager()
        {
            return View();
        }
    }

    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ReturnUrl { get; set; }
    
        public string Token { get; set; }
    }
}