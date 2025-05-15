using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace hyperTROPHYbuddy.Controllers.Admin
{
    public class WorkoutsController : Controller
    {
        private readonly IWorkoutService _workoutService;
        private readonly IExerciseService _exerciseService;

        public WorkoutsController(IWorkoutService workoutService, IExerciseService exerciseService)
        {
            _workoutService = workoutService;
            _exerciseService = exerciseService;
        }

        // GET: Admin/Workouts
        public async Task<IActionResult> Index()
        {
            var adminId = "hardcoded-admin-id";
            var workouts = await _workoutService.GetWorkoutsByAdmin(adminId);
            return View(workouts);
        }

        // GET: Admin/Workouts/Create
        public async Task<IActionResult> Create()
        {
            var adminId = "hardcoded-admin-id";
            var exercises = await _exerciseService.GetExercisesByAdmin(adminId);
            ViewBag.Exercises = new MultiSelectList(exercises, "Id", "Name");
            return View();
        }

        // POST: Admin/Workouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Workout workout, List<int> SelectedExerciseIds)
        {
            if (ModelState.IsValid)
            {
                workout.AdminId = "hardcoded-admin-id";
                await _workoutService.CreateWorkout(workout, SelectedExerciseIds);
                return RedirectToAction(nameof(Index));
            }
            return View(workout);
        }

        // GET: Admin/Workouts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var adminId = "hardcoded-admin-id";
            var workout = await _workoutService.GetWorkoutById(id, adminId);
            if (workout == null)
            {
                return NotFound();
            }

            var exercises = await _exerciseService.GetExercisesByAdmin(adminId);
            var selectedExercises = workout.WorkoutExercises.Select(we => we.ExerciseId).ToList();

            ViewBag.Exercises = new MultiSelectList(exercises, "Id", "Name", selectedExercises);
            return View(workout);
        }

        // POST: Admin/Workouts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Workout workout, List<int> SelectedExerciseIds)
        {
            if (id != workout.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                workout.AdminId = "hardcoded-admin-id";
                await _workoutService.UpdateWorkout(workout, SelectedExerciseIds);
                return RedirectToAction(nameof(Index));
            }
            return View(workout);
        }

        // GET: Admin/Workouts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var adminId = "hardcoded-admin-id";
            var workout = await _workoutService.GetWorkoutById(id, adminId);
            if (workout == null)
            {
                return NotFound();
            }
            return View(workout);
        }

        // POST: Admin/Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminId = "hardcoded-admin-id";
            await _workoutService.DeleteWorkout(id, adminId);
            return RedirectToAction(nameof(Index));
        }
    }
}
