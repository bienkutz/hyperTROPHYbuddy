using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
using Microsoft.AspNetCore.Mvc;

namespace hyperTROPHYbuddy.Controllers.Client
{
    public class WorkoutPlansController : Controller
    {
        private readonly IClientService _clientService;

        public WorkoutPlansController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // GET: User/WorkoutPlans
        public async Task<IActionResult> Index()
        {
            var clientId = "hardcoded-user-id"; // Replace with actual user ID later
            var plans = await _clientService.GetClientPlans(clientId);
            return View(plans);
        }

        // GET: User/WorkoutPlans/LogWorkout/5?workoutId=2
        public async Task<IActionResult> LogWorkout(int clientWorkoutPlanId, int workoutId)
        {
            var clientId = "hardcoded-user-id";
            var clientPlan = await _clientService.GetClientPlanById(clientWorkoutPlanId, clientId);
            if (clientPlan == null)
            {
                return NotFound();
            }

            var workout = clientPlan.WorkoutPlan.WorkoutPlanWorkouts
                .FirstOrDefault(wpw => wpw.WorkoutId == workoutId)?.Workout;

            if (workout == null)
            {
                return NotFound();
            }

            ViewBag.WorkoutId = workoutId;
            ViewBag.ClientWorkoutPlanId = clientWorkoutPlanId;
            ViewBag.Exercises = workout.WorkoutExercises.Select(we => we.Exercise).ToList();
            return View();
        }

        // POST: User/WorkoutPlans/LogWorkout/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogWorkout(
            int clientWorkoutPlanId,
            int workoutId,
            DateTime date,
            Dictionary<int, List<(int reps, decimal weight)>> exerciseSets)
        {
            var clientId = "hardcoded-user-id";
            var setLogs = new List<SetLog>();

            foreach (var (exerciseId, sets) in exerciseSets)
            {
                for (int i = 0; i < sets.Count; i++)
                {
                    setLogs.Add(new SetLog
                    {
                        ExerciseId = exerciseId,
                        SetNumber = i + 1,
                        Reps = sets[i].reps,
                        Weight = sets[i].weight
                    });
                }
            }

            try
            {
                await _clientService.LogWorkout(clientWorkoutPlanId, workoutId, clientId, date, setLogs);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(LogWorkout), new { clientWorkoutPlanId, workoutId });
            }
        }

        // GET: User/WorkoutPlans/History/5
        public async Task<IActionResult> History(int clientWorkoutPlanId)
        {
            var clientId = "hardcoded-user-id";
            var history = await _clientService.GetWorkoutHistory(clientWorkoutPlanId, clientId);
            return View(history);
        }
    }
}
