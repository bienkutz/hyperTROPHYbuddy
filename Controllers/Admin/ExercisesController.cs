using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
using Microsoft.AspNetCore.Mvc;

namespace hyperTROPHYbuddy.Controllers.Admin
{

    public class ExercisesController : Controller
    {
        private readonly IExerciseService _exerciseService;

        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        // GET: Admin/Exercises
        public async Task<IActionResult> Index()
        {
            var adminId = "hardcoded-admin-id"; // Replace with actual admin ID later
            var exercises = await _exerciseService.GetExercisesByAdmin(adminId);
            return View(exercises);
        }

        // GET: Admin/Exercises/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Exercises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                exercise.AdminId = "hardcoded-admin-id";
                await _exerciseService.CreateExercise(exercise);
                return RedirectToAction(nameof(Index));
            }
            return View(exercise);
        }

        // GET: Admin/Exercises/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var adminId = "hardcoded-admin-id";
            var exercise = await _exerciseService.GetExerciseById(id, adminId);
            if (exercise == null)
            {
                return NotFound();
            }
            return View(exercise);
        }

        // POST: Admin/Exercises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exercise exercise)
        {
            if (id != exercise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                exercise.AdminId = "hardcoded-admin-id";
                await _exerciseService.UpdateExercise(exercise);
                return RedirectToAction(nameof(Index));
            }
            return View(exercise);
        }

        // GET: Admin/Exercises/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var adminId = "hardcoded-admin-id";
            var exercise = await _exerciseService.GetExerciseById(id, adminId);
            if (exercise == null)
            {
                return NotFound();
            }
            return View(exercise);
        }

        // POST: Admin/Exercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminId = "hardcoded-admin-id";
            await _exerciseService.DeleteExercise(id, adminId);
            return RedirectToAction(nameof(Index));
        }
    }
}
