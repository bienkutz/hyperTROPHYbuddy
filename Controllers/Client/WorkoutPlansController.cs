using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
using Microsoft.AspNetCore.Mvc;

namespace hyperTROPHYbuddy.Controllers.Client
{
    public class WorkoutPlansController : BaseClientController
    {
        private readonly IClientService _clientService;

        public WorkoutPlansController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var plans = await _clientService.GetClientPlans(GetClientId());
                return View(plans);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }

        [HttpGet("LogWorkout")]
        public async Task<IActionResult> LogWorkout(int clientWorkoutPlanId, int workoutId)
        {
            try
            {
                var clientPlan = await _clientService.GetClientPlanById(clientWorkoutPlanId, GetClientId());
                if (clientPlan == null)
                    return NotFound();

                var workout = clientPlan.WorkoutPlan.WorkoutPlanWorkouts
                    .FirstOrDefault(wpw => wpw.WorkoutId == workoutId)?.Workout;

                if (workout == null)
                    return NotFound();

                ViewBag.WorkoutId = workoutId;
                ViewBag.ClientWorkoutPlanId = clientWorkoutPlanId;
                ViewBag.WorkoutName = workout.Name;
                ViewBag.Date = DateTime.Today;
                ViewBag.Exercises = workout.WorkoutExercises.Select(we => we.Exercise).ToList();

                return View();
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }

        [HttpPost("LogWorkout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogWorkout(int clientWorkoutPlanId, int workoutId, DateTime date, Dictionary<int, List<(int reps, decimal weight)>> exerciseSets)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction(nameof(LogWorkout), new { clientWorkoutPlanId, workoutId });
                }

                var workoutLog = new WorkoutLog
                {
                    ClientWorkoutPlanId = clientWorkoutPlanId,
                    WorkoutId = workoutId,
                    Date = date,
                    ClientId = GetClientId()
                };

                var setLogs = new List<SetLog>();
                foreach (var (exerciseId, sets) in exerciseSets)
                {
                    for (int i = 0; i < sets.Count; i++)
                    {
                        if (sets[i].reps > 0) // Only add sets with reps
                        {
                            setLogs.Add(new SetLog
                            {
                                ExerciseId = exerciseId,
                                SetNumber = i + 1,
                                Reps = sets[i].reps,
                                Weight = sets[i].weight,
                                ClientId = GetClientId()
                            });
                        }
                    }
                }

                await _clientService.LogWorkout(workoutLog, setLogs, GetClientId());
                TempData["Success"] = "Workout logged successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(LogWorkout), new { clientWorkoutPlanId, workoutId });
            }
        }

        [HttpGet("History/{clientWorkoutPlanId}")]
        public async Task<IActionResult> History(int clientWorkoutPlanId)
        {
            try
            {
                var history = await _clientService.GetWorkoutHistory(clientWorkoutPlanId, GetClientId());
                return View(history);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("WorkoutDetails/{workoutLogId}")]
        public async Task<IActionResult> WorkoutDetails(int workoutLogId)
        {
            try
            {
                var details = await _clientService.GetWorkoutDetails(workoutLogId, GetClientId());
                return View(details);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
