using hyperTROPHYbuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace hyperTROPHYbuddy.Controllers
{
    [Authorize]
    public class HealthReportController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public HealthReportController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // Map activity level labels to multipliers
        private static readonly Dictionary<string, double> ActivityMultipliers = new Dictionary<string, double>
        {
            { "Light", 1.375 },
            { "Moderate", 1.55 },
            { "Active", 1.725 },
            { "Very Active", 1.9 }
        };

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                RedirectToAction("Index", "Home");
            }
            ViewBag.ActivityLevels = GetActivityLevels();
            return View(new HealthReportInputModel());
        }

        [HttpPost]
        public async Task <IActionResult> Index(HealthReportInputModel input)
        {
            
            var user = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                RedirectToAction("Index", "Home");
            }

            ViewBag.ActivityLevels = GetActivityLevels();

            // Parse weight with support for both '.' and ',' as decimal separator
            double weight;
            var weightString = (input.Weight ?? "").Replace(',', '.');
            if (!double.TryParse(weightString, NumberStyles.Any, CultureInfo.InvariantCulture, out weight))
            {
                ModelState.AddModelError("Weight", "Invalid weight. Use numbers with . or , as decimal separator.");
                return View(input);
            }

            if (!ModelState.IsValid || !ActivityMultipliers.ContainsKey(input.ActivityLevel))
                return View(input);

            double heightM = input.Height / 100.0;
            double bmi = weight / (heightM * heightM);
            string bmiCategory = GetBMICategory(bmi);

            double bmr = input.Gender.ToLower() == "male"
                ? 10 * weight + 6.25 * input.Height - 5 * input.Age + 5
                : 10 * weight + 6.25 * input.Height - 5 * input.Age - 161;

            double multiplier = ActivityMultipliers[input.ActivityLevel];
            double maintainCalories = bmr * multiplier;

            // 1 kg fat ≈ 7700 kcal
            double mildLossCalories = maintainCalories - (0.25 * 7700 / 7);   // -0.25 kg/week
            double lossCalories = maintainCalories - (0.5 * 7700 / 7);        // -0.5 kg/week
            double mildGainCalories = maintainCalories + (0.25 * 7700 / 7);   // +0.25 kg/week
            double gainCalories = maintainCalories + (0.5 * 7700 / 7);        // +0.5 kg/week

            var result = new HealthReportResultModel
            {
                BMI = Math.Round(bmi, 2),
                BMICategory = bmiCategory,
                MaintainCalories = Math.Round(maintainCalories),
                MildLossCalories = Math.Round(mildLossCalories),
                LossCalories = Math.Round(lossCalories),
                MildGainCalories = Math.Round(mildGainCalories),
                GainCalories = Math.Round(gainCalories)
            };

            ViewBag.Result = result;
            return View(input);
        }

        private Dictionary<string, string> GetActivityLevels()
        {
            return new Dictionary<string, string>
            {
                { "Light", "Light (exercise 1-3 times/week)" },
                { "Moderate", "Moderate (exercise 4-5 times/week)" },
                { "Active", "Active (daily exercise or intense exercise 3-4 times/week)" },
                { "Very Active", "Very Active (intense exercise 6-7 times/week)" }
            };
        }

        private string GetBMICategory(double bmi)
        {
            if (bmi < 18.5) return "Underweight";
            if (bmi < 25) return "Normal weight";
            if (bmi < 30) return "Overweight";
            return "Obese";
        }
    }
}
