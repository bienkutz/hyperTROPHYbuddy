using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.EntityFrameworkCore;
namespace hyperTROPHYbuddy.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;

        public ClientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClientWorkoutPlan>> GetClientPlans(string clientId)
        {
            return await _context.ClientWorkoutPlans
                .Where(cwp => cwp.ClientId == clientId)
                .Include(cwp => cwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanType)
                .Include(cwp => cwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanWorkouts)
                .ThenInclude(wpw => wpw.Workout)
                .ThenInclude(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .ToListAsync();
        }

        public async Task<ClientWorkoutPlan> GetClientPlanById(int id, string clientId)
        {
            return await _context.ClientWorkoutPlans
                .Include(cwp => cwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanType)
                .Include(cwp => cwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanWorkouts)
                .ThenInclude(wpw => wpw.Workout)
                .ThenInclude(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .FirstOrDefaultAsync(cwp => cwp.Id == id && cwp.ClientId == clientId);
        }

        public async Task LogWorkout(int clientWorkoutPlanId, int workoutId, string clientId, DateTime date, List<SetLog> setLogs)
        {
            // Validate the user has access to this plan
            var clientPlan = await _context.ClientWorkoutPlans
                .Include(cwp => cwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanWorkouts)
                .FirstOrDefaultAsync(cwp => cwp.Id == clientWorkoutPlanId && cwp.ClientId == clientId);

            if (clientPlan == null)
            {
                throw new InvalidOperationException("Workout plan not found or doesn't belong to you.");
            }

            // Validate the workout is part of the plan
            if (!clientPlan.WorkoutPlan.WorkoutPlanWorkouts.Any(wpw => wpw.WorkoutId == workoutId))
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
                ClientWorkoutPlanId = clientWorkoutPlanId,
                WorkoutId = workoutId,
                Date = date,
                SetLogs = setLogs
            };

            _context.WorkoutLogs.Add(workoutLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<WorkoutLog>> GetWorkoutHistory(int clientWorkoutPlanId, string clientId)
        {
            return await _context.WorkoutLogs
                .Where(wl => wl.ClientWorkoutPlanId == clientWorkoutPlanId && wl.ClientWorkoutPlan.ClientId == clientId)
                .Include(wl => wl.Workout)
                .Include(wl => wl.SetLogs)
                .ThenInclude(sl => sl.Exercise)
                .OrderByDescending(wl => wl.Date)
                .ToListAsync();
        }
    }
}
