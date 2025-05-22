using hyperTROPHYbuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Exercises", new { area = "Admin" });
            }

            if (User.IsInRole("Client"))
            {
                return RedirectToAction("Index", "WorkoutPlans", new { area = "Client" });
            }
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}