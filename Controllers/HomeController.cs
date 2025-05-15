using System.Diagnostics;
using hyperTROPHYbuddy.Models;
using Microsoft.AspNetCore.Mvc;

/*namespace hyperTROPHYbuddy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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
*/

namespace hyperTROPHYbuddy.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Temporary role selection for testing
            //ViewBag.IsAdmin = true; // Change to false to test user view
            return View();
        }

        [HttpPost]
        public IActionResult SetRole(bool isAdmin)
        {
            // For testing without real auth
            //HttpContext.Items["IsAdmin"] = isAdmin;
            return RedirectToAction("Index");
        }
    }
}


