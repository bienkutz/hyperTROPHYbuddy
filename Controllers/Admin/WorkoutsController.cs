using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace hyperTROPHYbuddy.Controllers.Admin
{
    public class WorkoutsController : BaseAdminController
    {
        private readonly IWorkoutService _workoutService;
        private readonly IExerciseService _exerciseService;

        public WorkoutsController(IWorkoutService workoutService, IExerciseService exerciseService)
        {
            _workoutService = workoutService;
            _exerciseService = exerciseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm)
        {
            try
            {
                var workouts = string.IsNullOrEmpty(searchTerm)
                    ? await _workoutService.GetWorkoutsByAdmin(GetAdminId())
                    : await _workoutService.SearchWorkouts(searchTerm, GetAdminId());
                return View(workouts);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var exercises = await _exerciseService.GetExercisesByAdmin(GetAdminId());
            ViewBag.Exercises = new MultiSelectList(exercises, "Id", "Name");
            return View(new Workout());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Workout workout, List<int> selectedExerciseIds)
        {
            if (!ModelState.IsValid)
            {
                var exercises = await _exerciseService.GetExercisesByAdmin(GetAdminId());
                ViewBag.Exercises = new MultiSelectList(exercises, "Id", "Name", selectedExerciseIds);
                return View(workout);
            }

            try
            {
                await _workoutService.CreateWorkout(workout, selectedExerciseIds, GetAdminId());
                TempData["Success"] = "Workout created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var exercises = await _exerciseService.GetExercisesByAdmin(GetAdminId());
                ViewBag.Exercises = new MultiSelectList(exercises, "Id", "Name", selectedExerciseIds);
                return View(workout);
            }
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var workout = await _workoutService.GetWorkoutById(id, GetAdminId());
                var exercises = await _exerciseService.GetExercisesByAdmin(GetAdminId());
                var selectedExercises = workout.WorkoutExercises.Select(we => we.ExerciseId).ToList();

                ViewBag.Exercises = new MultiSelectList(exercises, "Id", "Name", selectedExercises);
                return View(workout);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Workout workout, List<int> selectedExerciseIds)
        {
            if (id != workout.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var exercises = await _exerciseService.GetExercisesByAdmin(GetAdminId());
                ViewBag.Exercises = new MultiSelectList(exercises, "Id", "Name", selectedExerciseIds);
                return View(workout);
            }

            try
            {
                await _workoutService.UpdateWorkout(workout, selectedExerciseIds, GetAdminId());
                TempData["Success"] = "Workout updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var exercises = await _exerciseService.GetExercisesByAdmin(GetAdminId());
                ViewBag.Exercises = new MultiSelectList(exercises, "Id", "Name", selectedExerciseIds);
                return View(workout);
            }
        }

        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var workout = await _workoutService.GetWorkoutById(id, GetAdminId());
                return View(workout);
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
                await _workoutService.DeleteWorkout(id, GetAdminId());
                TempData["Success"] = "Workout deleted successfully";
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

