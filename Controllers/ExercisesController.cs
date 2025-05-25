using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Controllers
{
    public class ExercisesController : Controller
    {
        private readonly IExerciseService _exerciseService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExercisesController(IExerciseService exerciseService, UserManager<ApplicationUser> userManager)
        {
            _exerciseService = exerciseService;
            _userManager = userManager;
        }

        // GET: Exercises
        public async Task<IActionResult> Index()
        {
            var exercises = await _exerciseService.GetAllAsync();
            return View(exercises);
        }

        // GET: Exercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var exercise = await _exerciseService.GetByIdAsync(id.Value);
            if (exercise == null)
                return NotFound();

            return View(exercise);
        }

        // GET: Exercises/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Exercises/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExerciseId,Name,Description,VideoLink")] Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                exercise.CreatedByAdminId = currentUser.Id;

                await _exerciseService.CreateAsync(exercise);
                return RedirectToAction(nameof(Index));
            }
            
            return View(exercise);
        }

        // GET: Exercises/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var exercise = await _exerciseService.GetByIdAsync(id.Value);
            if (exercise == null)
                return NotFound();

            ViewData["CreatedByAdminId"] = new SelectList(_userManager.Users.ToList(), "Id", "UserName", exercise.CreatedByAdminId);
            return View(exercise);
        }

        // POST: Exercises/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExerciseId,Name,Description,VideoLink")] Exercise exercise)
        {
            if (id != exercise.ExerciseId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _exerciseService.UpdateAsync(exercise);
                }
                catch
                {
                    var exists = await _exerciseService.GetByIdAsync(exercise.ExerciseId);
                    if (exists == null)
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedByAdminId"] = new SelectList(_userManager.Users.ToList(), "Id", "UserName", exercise.CreatedByAdminId);
            return View(exercise);
        }

        // GET: Exercises/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var exercise = await _exerciseService.GetByIdAsync(id.Value);
            if (exercise == null)
                return NotFound();

            return View(exercise);
        }

        // POST: Exercises/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _exerciseService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}