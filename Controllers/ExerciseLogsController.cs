using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using hyperTROPHYbuddy.Data;
using System.Collections.Generic;

namespace hyperTROPHYbuddy.Controllers
{
    public class ExerciseLogsController : Controller
    {
        private readonly IExerciseService _exerciseService;
        private readonly IWorkoutService _workoutService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context; // Use concrete ApplicationDbContext

        public ExerciseLogsController(
            IExerciseService exerciseService,
            IWorkoutService workoutService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context) // Use ApplicationDbContext here
        {
            _exerciseService = exerciseService;
            _workoutService = workoutService;
            _userManager = userManager;
            _context = context;
        }

        // GET: ExerciseLogs/Create?workoutId=1
        public async Task<IActionResult> Create(int workoutId)
        {
            var workout = await _workoutService.GetByIdAsync(workoutId);
            if (workout == null) return NotFound();

            ViewBag.Workout = workout;
            ViewBag.Exercises = workout.WorkoutExercises.Select(we => we.Exercise).ToList();
            return View();
        }

        // POST: ExerciseLogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int workoutId, List<List<ExerciseLogInputModel>> logs)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Flatten the list of lists
            var flatLogs = logs?.SelectMany(x => x) ?? Enumerable.Empty<ExerciseLogInputModel>();

            // Server-side: group by ExerciseId and check for >5 sets
            var overLimit = flatLogs
                .GroupBy(l => l.ExerciseId)
                .Any(g => g.Count() > 5);

            if (overLimit)
            {
                ModelState.AddModelError("", "You cannot log more than 5 sets per exercise.");
                // Optionally, reload the workout/exercises for redisplay
                var workout = await _workoutService.GetByIdAsync(workoutId);
                ViewBag.Workout = workout;
                ViewBag.Exercises = workout.WorkoutExercises.Select(we => we.Exercise).ToList();
                return View();
            }

            foreach (var log in flatLogs)
            {
                var entry = new ExerciseLog
                {
                    UserId = user.Id,
                    WorkoutId = workoutId,
                    ExerciseId = log.ExerciseId,
                    SetNumber = log.SetNumber,
                    Reps = log.Reps,
                    Weight = log.Weight,
                    LoggedAt = DateTime.Now
                };
                _context.Add(entry);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("MyPlan", "WorkoutPlanAssignments");
        }

        // GET: ExerciseLogs/Index
        public async Task<IActionResult> Index(int? workoutId, int? exerciseId)
        {
            var user = await _userManager.GetUserAsync(User);
            var logsQuery = _context.Set<ExerciseLog>()
                .Include(l => l.Exercise)
                .Include(l => l.Workout)
                .Where(l => l.UserId == user.Id);

            if (workoutId.HasValue)
            {
                logsQuery = logsQuery.Where(l => l.WorkoutId == workoutId.Value);
                var workout = await _workoutService.GetByIdAsync(workoutId.Value);
                ViewBag.FilteredWorkoutName = workout?.Name;
                // Provide exercises for filter dropdown
                ViewBag.Exercises = workout?.WorkoutExercises.Select(we => we.Exercise).ToList();
            }

            // Group logs by Workout and LoggedAt (date + hour + minute)
            var groupedLogs = logsQuery
                .AsEnumerable()
                .GroupBy(l => new
                {
                    l.WorkoutId,
                    DateTime = new DateTime(
                        l.LoggedAt.Year,
                        l.LoggedAt.Month,
                        l.LoggedAt.Day,
                        l.LoggedAt.Hour,
                        l.LoggedAt.Minute,
                        0)
                })
                .Select(g => new
                {
                    WorkoutId = g.Key.WorkoutId,
                    DateTime = g.Key.DateTime,
                    WorkoutName = g.First().Workout?.Name,
                    LogCount = g.Count()
                })
                .OrderByDescending(g => g.DateTime)
                .ToList();

            return View("GroupedIndex", groupedLogs);
        }
        //Details Action To Show All Logs For A Workout On A Specific Date
        public async Task<IActionResult> DetailsByDate(int workoutId, DateTime date, int? exerciseId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var logs = _context.Set<ExerciseLog>()
                .Include(l => l.Exercise)
                .Include(l => l.Workout)
                .Where(l =>
                    l.UserId == user.Id &&
                    l.WorkoutId == workoutId &&
                    l.LoggedAt.Year == date.Year &&
                    l.LoggedAt.Month == date.Month &&
                    l.LoggedAt.Day == date.Day &&
                    l.LoggedAt.Hour == date.Hour &&
                    l.LoggedAt.Minute == date.Minute
                )
                .OrderBy(l => l.Exercise.Name).ThenBy(l => l.SetNumber)
                .ToList();

            ViewBag.Date = date;
            ViewBag.WorkoutName = logs.FirstOrDefault()?.Workout?.Name;

            // Populate exercises for filter dropdown
            ViewBag.Exercises = logs
                .Select(l => l.Exercise)
                .Where(e => e != null)
                .Distinct()
                .OrderBy(e => e.Name)
                .ToList();

            return View("DetailsByDate", logs);
        }
    }

    // Helper input model for logging multiple sets
    public class ExerciseLogInputModel
    {
        public int ExerciseId { get; set; }
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }
    }
}