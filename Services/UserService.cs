using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.EntityFrameworkCore;
namespace hyperTROPHYbuddy.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserWorkoutPlan>> GetUserPlans(string userId)
        {
            return await _context.UserWorkoutPlans
                .Where(uwp => uwp.UserId == userId)
                .Include(uwp => uwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanType)
                .Include(uwp => uwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanWorkouts)
                .ThenInclude(wpw => wpw.Workout)
                .ThenInclude(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .ToListAsync();
        }

        public async Task<UserWorkoutPlan> GetUserPlanById(int id, string userId)
        {
            return await _context.UserWorkoutPlans
                .Include(uwp => uwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanType)
                .Include(uwp => uwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanWorkouts)
                .ThenInclude(wpw => wpw.Workout)
                .ThenInclude(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .FirstOrDefaultAsync(uwp => uwp.Id == id && uwp.UserId == userId);
        }

        public async Task LogWorkout(int userWorkoutPlanId, int workoutId, string userId, DateTime date, List<SetLog> setLogs)
        {
            // Validate the user has access to this plan
            var userPlan = await _context.UserWorkoutPlans
                .Include(uwp => uwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanWorkouts)
                .FirstOrDefaultAsync(uwp => uwp.Id == userWorkoutPlanId && uwp.UserId == userId);

            if (userPlan == null)
            {
                throw new InvalidOperationException("Workout plan not found or doesn't belong to you.");
            }

            // Validate the workout is part of the plan
            if (!userPlan.WorkoutPlan.WorkoutPlanWorkouts.Any(wpw => wpw.WorkoutId == workoutId))
            {
                throw new InvalidOperationException("Workout is not part of this plan.");
            }

            // Validate set logs (max 3 sets per exercise)
            var exercisesInWorkout = await _context.WorkoutExercises
                .Where(we => we.WorkoutId == workoutId)
                .Select(we => we.ExerciseId)
                .ToListAsync();

            foreach (var exerciseId in exercisesInWorkout)
            {
                var exerciseSets = setLogs.Where(sl => sl.ExerciseId == exerciseId).ToList();
                if (exerciseSets.Count > 3)
                {
                    throw new InvalidOperationException($"Exercise with ID {exerciseId} has more than 3 sets.");
                }
            }

            // Create workout log
            var workoutLog = new WorkoutLog
            {
                UserWorkoutPlanId = userWorkoutPlanId,
                WorkoutId = workoutId,
                Date = date,
                SetLogs = setLogs
            };

            _context.WorkoutLogs.Add(workoutLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<WorkoutLog>> GetWorkoutHistory(int userWorkoutPlanId, string userId)
        {
            return await _context.WorkoutLogs
                .Where(wl => wl.UserWorkoutPlanId == userWorkoutPlanId && wl.UserWorkoutPlan.UserId == userId)
                .Include(wl => wl.Workout)
                .Include(wl => wl.SetLogs)
                .ThenInclude(sl => sl.Exercise)
                .OrderByDescending(wl => wl.Date)
                .ToListAsync();
        }
    }
}
