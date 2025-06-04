using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Controllers
{
    [Authorize(Roles = "Client")]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        // Spoonacular supported diets (static for now, can be fetched from API if needed)
        private static readonly List<string> SpoonacularDiets = new List<string>
        {
            "", // For "No preference"
            "Gluten Free",
            "Ketogenic",
            "Vegetarian",
            "Lacto-Vegetarian",
            "Ovo-Vegetarian",
            "Vegan",
            "Pescetarian",
            "Paleo",
            "Primal",
            "Low FODMAP",
            "Whole30"
        };


        // GET: UserProfile/Edit/{userId}
        public async Task<IActionResult> Edit()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.DietOptions = SpoonacularDiets;

            var user = await _db.UserProfiles
                .Include(u => u.Allergies)
                .FirstOrDefaultAsync(u => u.UserId == currentUser.Id);

            if (user == null)
                return View(new UserProfileModel { UserId = currentUser.Id });

            return View(new UserProfileModel
            {
                UserId = user.UserId,
                DietPreference = user.DietPreference,
                Allergies = string.Join(", ", user.Allergies.Select(a => a.Allergy))
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserProfileModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            model.UserId = currentUser.Id;
            ViewBag.DietOptions = SpoonacularDiets;

            var user = await _db.UserProfiles
                .Include(u => u.Allergies)
                .FirstOrDefaultAsync(u => u.UserId == model.UserId);

            if (user == null)
            {
                user = new UserProfile { UserId = model.UserId };
                _db.UserProfiles.Add(user);
            }

            // Allow null/empty diet preference
            user.DietPreference = string.IsNullOrWhiteSpace(model.DietPreference) ? null : model.DietPreference;

            // Allow null/empty allergies
            user.Allergies.Clear();
            if (!string.IsNullOrWhiteSpace(model.Allergies))
            {
                foreach (var allergy in model.Allergies.Split(',', System.StringSplitOptions.RemoveEmptyEntries))
                    user.Allergies.Add(new UserAllergy { Allergy = allergy.Trim(), UserId = user.UserId });
            }
            await _db.SaveChangesAsync();

            // Reload to show updated allergies
            return RedirectToAction(nameof(Edit));
        }
    }

    public class UserProfileModel
    {
        public string UserId { get; set; }
        public string DietPreference { get; set; }
        public string Allergies { get; set; }
    }
}