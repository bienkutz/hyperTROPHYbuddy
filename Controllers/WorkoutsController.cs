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
        public async Task<IActionResult> Index()
        {
            var workouts = await _workoutService.GetAllAsync();
            return View(workouts);
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

            ViewData["CreatedByAdminId"] = new SelectList(_userManager.Users.ToList(), "Id", "UserName", workout.CreatedByAdminId);
            return View(workout);
        }

        // POST: Workouts/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkoutId,Name,CreatedByAdminId")] Workout workout)
        {
            if (id != workout.WorkoutId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _workoutService.UpdateAsync(workout);
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
            ViewData["CreatedByAdminId"] = new SelectList(_userManager.Users.ToList(), "Id", "UserName", workout.CreatedByAdminId);
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