using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
using Microsoft.AspNetCore.Mvc;

namespace hyperTROPHYbuddy.Controllers.Admin
{

    public class ExercisesController : BaseAdminController
    {
        private readonly IExerciseService _exerciseService;

        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            try
            {
                var exercises = string.IsNullOrEmpty(searchTerm)
                    ? await _exerciseService.GetExercisesByAdmin(GetAdminId())
                    : await _exerciseService.SearchExercises(searchTerm, GetAdminId());
                return View(exercises);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(new Exercise());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exercise exercise)
        {
            if (!ModelState.IsValid)
                return View(exercise);

            try
            {
                await _exerciseService.CreateExercise(exercise, GetAdminId());
                TempData["Success"] = "Exercise created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(exercise);
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var exercise = await _exerciseService.GetExerciseById(id, GetAdminId());
                return View(exercise);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exercise exercise)
        {
            if (id != exercise.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(exercise);

            try
            {
                await _exerciseService.UpdateExercise(exercise, GetAdminId());
                TempData["Success"] = "Exercise updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(exercise);
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var exercise = await _exerciseService.GetExerciseById(id, GetAdminId());
                return View(exercise);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _exerciseService.DeleteExercise(id, GetAdminId());
                TempData["Success"] = "Exercise deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

