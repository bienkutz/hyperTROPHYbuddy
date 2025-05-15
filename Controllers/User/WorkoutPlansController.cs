using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
using Microsoft.AspNetCore.Mvc;

namespace hyperTROPHYbuddy.Controllers.User
{
    public class WorkoutPlansController : Controller
    {
        private readonly IUserService _userService;

        public WorkoutPlansController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: User/WorkoutPlans
        public async Task<IActionResult> Index()
        {
            var userId = "hardcoded-user-id"; // Replace with actual user ID later
            var plans = await _userService.GetUserPlans(userId);
            return View(plans);
        }

        // GET: User/WorkoutPlans/LogWorkout/5?workoutId=2
        public async Task<IActionResult> LogWorkout(int userWorkoutPlanId, int workoutId)
        {
            var userId = "hardcoded-user-id";
            var userPlan = await _userService.GetUserPlanById(userWorkoutPlanId, userId);
            if (userPlan == null)
            {
                return NotFound();
            }

            var workout = userPlan.WorkoutPlan.WorkoutPlanWorkouts
                .FirstOrDefault(wpw => wpw.WorkoutId == workoutId)?.Workout;

            if (workout == null)
            {
                return NotFound();
            }

            ViewBag.WorkoutId = workoutId;
            ViewBag.UserWorkoutPlanId = userWorkoutPlanId;
            ViewBag.Exercises = workout.WorkoutExercises.Select(we => we.Exercise).ToList();
            return View();
        }

        // POST: User/WorkoutPlans/LogWorkout/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogWorkout(
            int userWorkoutPlanId,
            int workoutId,
            DateTime date,
            Dictionary<int, List<(int reps, decimal weight)>> exerciseSets)
        {
            var userId = "hardcoded-user-id";
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
                await _userService.LogWorkout(userWorkoutPlanId, workoutId, userId, date, setLogs);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(LogWorkout), new { userWorkoutPlanId, workoutId });
            }
        }

        // GET: User/WorkoutPlans/History/5
        public async Task<IActionResult> History(int userWorkoutPlanId)
        {
            var userId = "hardcoded-user-id";
            var history = await _userService.GetWorkoutHistory(userWorkoutPlanId, userId);
            return View(history);
        }
    }
}
