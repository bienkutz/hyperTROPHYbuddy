using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
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
    public class MealPlanController : Controller
    {
        private const string MealCacheKey = "MealCache";
        private const int BatchSize = 10; // Number of recipes to cache per batch

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly SpoonacularMealPlannerService _planner;

        public MealPlanController(ApplicationDbContext db, SpoonacularMealPlannerService planner, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _planner = planner;
            _userManager = userManager;
        }

        // GET: MealPlan/Request
        public async Task<IActionResult> Request(int? mealCalories = null, string mealType = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var model = new SingleMealRequestViewModel
            {
                UserId = currentUser.Id // Set, but do not show/edit in view
            };
            if (mealCalories.HasValue)
                model.MealCalories = mealCalories.Value;
            if (!string.IsNullOrEmpty(mealType))
                model.MealType = mealType;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Request(SingleMealRequestViewModel model, bool generateAnother = false)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            model.UserId = currentUser.Id;

            if (model.MealCalories <= 0)
            {
                ViewBag.Message = "Meal Calories are required.";
                return View(model);
            }

            var user = await _db.UserProfiles
                .Include(u => u.Allergies)
                .Include(u => u.MealFeedbacks)
                .FirstOrDefaultAsync(u => u.UserId == model.UserId);
            if (user == null)
            {
                ViewBag.Message = "User not found.";
                return View(model);
            }

            // 1. Get cached recipe IDs from session
            var cache = HttpContext.Session.GetObject<List<int>>(MealCacheKey) ?? new List<int>();

            // 2. If cache is empty, fetch a new batch and cache it
            if (cache.Count == 0)
            {
                var batch = await _planner.GetRecipeBatchAsync(
                    model.MealCalories,
                    string.IsNullOrWhiteSpace(model.MealType) ? null : model.MealType,
                    user.DietPreference,
                    user.Allergies.Select(a => a.Allergy).ToList(),
                    BatchSize
                );
                cache = batch.Select(r => r.Id).ToList();
                HttpContext.Session.SetObject(MealCacheKey, cache);
            }

            // 3. Pick the next recipe ID and remove it from cache
            var nextRecipeId = cache.First();
            cache.RemoveAt(0);
            HttpContext.Session.SetObject(MealCacheKey, cache);

            // 4. Fetch recipe details (single API call)
            var meal = await _planner.GetRecipeDetailsAsync(nextRecipeId);

            if (meal == null)
            {
                ViewBag.Message = "No meal found.";
                return View(model);
            }

            var mealVm = new MealWithFeedbackViewModel
            {
                MealId = meal.Id.ToString(),
                Label = meal.Title,
                Image = $"https://spoonacular.com/recipeImages/{meal.Id}-312x231.{meal.ImageType}",
                Calories = meal.Nutrition?.Nutrients?.FirstOrDefault(n => n.Name == "Calories")?.Amount ?? 0,
                Ingredients = meal.ExtendedIngredients?.Select(i => i.OriginalString).ToList() ?? new List<string>(),
                Url = meal.SourceUrl,
                Feedback = user.MealFeedbacks.FirstOrDefault(f => f.MealId == meal.Id.ToString())?.Liked
            };

            model.Meal = mealVm;
            return View("SingleMeal", model);
        }

        [HttpPost]
        public async Task<IActionResult> Feedback(string mealId, bool liked, int mealCalories, string mealType)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userId = currentUser.Id;

            var user = await _db.UserProfiles
                .Include(u => u.MealFeedbacks)
                .Include(u => u.Allergies)
                .FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                ViewBag.Message = "User not found.";
                return View("SingleMeal", new SingleMealRequestViewModel { UserId = userId });
            }
            var feedback = user.MealFeedbacks.FirstOrDefault(f => f.MealId == mealId);
            if (feedback == null)
                user.MealFeedbacks.Add(new MealFeedback { MealId = mealId, UserId = userId, Liked = liked });
            else
                feedback.Liked = liked;

            await _db.SaveChangesAsync();

            // Fetch meal details to display the same meal
            var meal = await _planner.GetRecipeDetailsAsync(int.Parse(mealId));
            var mealVm = new MealWithFeedbackViewModel
            {
                MealId = meal.Id.ToString(),
                Label = meal.Title,
                Image = $"https://spoonacular.com/recipeImages/{meal.Id}-312x231.{meal.ImageType}",
                Calories = meal.Nutrition?.Nutrients?.FirstOrDefault(n => n.Name == "Calories")?.Amount ?? 0,
                Ingredients = meal.ExtendedIngredients?.Select(i => i.OriginalString).ToList() ?? new List<string>(),
                Url = meal.SourceUrl,
                Feedback = liked
            };

            var model = new SingleMealRequestViewModel
            {
                UserId = userId,
                MealCalories = mealCalories,
                MealType = mealType,
                Meal = mealVm
            };

            ViewBag.Message = "Feedback saved!";
            return View("SingleMeal", model);
        }
    }

    public class SingleMealRequestViewModel
    {
        public string UserId { get; set; }
        public int MealCalories { get; set; }
        public string MealType { get; set; }
        public MealWithFeedbackViewModel Meal { get; set; }
    }

    public class MealWithFeedbackViewModel
    {
        public string MealId { get; set; }
        public string Label { get; set; }
        public string Image { get; set; }
        public double Calories { get; set; }
        public List<string> Ingredients { get; set; }
        public string Url { get; set; }
        public bool? Feedback { get; set; }
    }
}