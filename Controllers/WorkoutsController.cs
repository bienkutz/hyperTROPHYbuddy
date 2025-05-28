using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly IWorkoutService _workoutService;
        private readonly IExerciseService _exerciseService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWorkoutExerciseService _workoutExerciseService;

        public WorkoutsController(
            IWorkoutService workoutService,
            UserManager<ApplicationUser> userManager,
            IExerciseService exerciseService,
            IWorkoutExerciseService workoutExerciseService)
        {
            _workoutService = workoutService;
            _userManager = userManager;
            _exerciseService = exerciseService;
            _workoutExerciseService = workoutExerciseService;
        }

        // GET: Workouts
        public async Task<IActionResult> Index(string search)
        {
            var workouts = await _workoutService.GetAllAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                workouts = workouts.Where(w => w.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            ViewBag.Search = search;
            return View(workouts);
        }

        [HttpGet]
        public async Task<IActionResult> Suggest(string term)
        {
            var workouts = await _workoutService.GetAllAsync();
            var suggestions = workouts
                .Where(w => !string.IsNullOrEmpty(term) && w.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(w => w.Name)
                .Distinct()
                .Take(10)
                .ToList();
            return Json(suggestions);
        }

        // GET: Workouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var workout = await _workoutService.GetByIdAsync(id.Value);
            if (workout == null)
                return NotFound();

            return View(workout);
        }

        // GET: Workouts/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var exercises = await _exerciseService.GetAllAsync();
            ViewBag.Exercises = exercises.ToList();
            return View();
        }

        // POST: Workouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name")] Workout workout, int[] SelectedExerciseIds)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                workout.CreatedByAdminId = currentUser.Id;
                await _workoutService.CreateAsync(workout);

                // Add WorkoutExercise mappings
                if (SelectedExerciseIds != null && SelectedExerciseIds.Length > 0)
                {
                    foreach (var exId in SelectedExerciseIds)
                    {
                        var workoutExercise = new WorkoutExercise
                        {
                            WorkoutId = workout.WorkoutId,
                            ExerciseId = exId
                        };
                        // Use your WorkoutExerciseService or DbContext here
                        // Example:
                        await _workoutExerciseService.AddAsync(workoutExercise);
                        // or, if using DbContext directly:
                        //_context.Add(workoutExercise);
                    }
                    //await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            // Repopulate exercises if model state is invalid
            ViewBag.Exercises = (await _exerciseService.GetAllAsync()).ToList();
            return View(workout);
        }

        // GET: Workouts/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var workout = await _workoutService.GetByIdAsync(id.Value);
            if (workout == null)
                return NotFound();

            // Get all exercises
            var allExercises = (await _exerciseService.GetAllAsync()).ToList();

            // Get selected exercise IDs for this workout
            var selectedExerciseIds = (await _workoutExerciseService.GetByWorkoutIdAsync(workout.WorkoutId))
                .Select(we => we.ExerciseId)
                .ToList();

            ViewBag.Exercises = allExercises;
            ViewBag.SelectedExerciseIds = selectedExerciseIds;

            return View(workout);
        }

        // POST: Workouts/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkoutId,Name")] Workout workout, int[] SelectedExerciseIds)
        {
            if (id != workout.WorkoutId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingWorkout = await _workoutService.GetByIdAsync(id);
                    if (existingWorkout == null)
                        return NotFound();

                    existingWorkout.Name = workout.Name;
                    // Do NOT touch existingWorkout.CreatedByAdminId

                    await _workoutService.UpdateAsync(existingWorkout);

                    // Remove and re-add exercises as before...
                    await _workoutExerciseService.RemoveAllForWorkoutAsync(existingWorkout.WorkoutId);
                    if (SelectedExerciseIds != null && SelectedExerciseIds.Length > 0)
                    {
                        foreach (var exId in SelectedExerciseIds)
                        {
                            var workoutExercise = new WorkoutExercise
                            {
                                WorkoutId = existingWorkout.WorkoutId,
                                ExerciseId = exId
                            };
                            await _workoutExerciseService.AddAsync(workoutExercise);
                        }
                    }
                }
                catch
                {
                    var exists = await _workoutService.GetByIdAsync(workout.WorkoutId);
                    if (exists == null)
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Repopulate exercises if model state is invalid
            ViewBag.Exercises = (await _exerciseService.GetAllAsync()).ToList();
            ViewBag.SelectedExerciseIds = SelectedExerciseIds?.ToList() ?? new List<int>();
            return View(workout);
        }

        // GET: Workouts/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var workout = await _workoutService.GetByIdAsync(id.Value);
            if (workout == null)
                return NotFound();

            return View(workout);
        }

        // POST: Workouts/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _workoutService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}