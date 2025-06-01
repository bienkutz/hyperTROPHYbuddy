using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Mono.TextTemplating;
using System.Diagnostics;

namespace hyperTROPHYbuddy.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApplicationUserService _userService;
        private const string AdminPassword = "Admin123@";

        public HomeController(IApplicationUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Route("/login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> Login(ApplicationUser user)
        {
            if (!ModelState.IsValid) return View(user);
            var result = await _userService.Login(user.UserName, user.PasswordHash, false, false);
            if (result.Succeeded)
            {
                return Redirect("/home");
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(user);
        }

        [HttpGet]
        [Route("/register")]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> SignUp(ApplicationUser user, string? Role, string? AdminPassword)
        {
            if (!ModelState.IsValid) return View(user);
            var roles = new List<string> { "Client" };

            if (!string.IsNullOrEmpty(Role))
            {
                // Admin role was requested, verify the admin password
                if (string.IsNullOrEmpty(AdminPassword) || AdminPassword != HomeController.AdminPassword)
                {
                    // Invalid admin password
                    ModelState.AddModelError("", "Invalid admin password.");
                    ViewData["AdminPasswordError"] = "The admin password is incorrect.";
                    return View(user);
                }

                // Admin password is correct, add the Admin role
                roles.Add("Admin");
            }

            var result = await _userService.Register(user, user.PasswordHash, roles);

            if (result.Succeeded)
            {
                return Redirect("/login");
            }

            return View(user);
        }

        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout();
            return Redirect("/login");
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
}
