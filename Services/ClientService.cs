using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.EntityFrameworkCore;


namespace hyperTROPHYbuddy.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationManagerService _authorizationManagerService;

        public ClientService(
            ApplicationDbContext context,
            IAuthorizationManagerService authorizationManagerService)
        {
            _context = context;
            _authorizationManagerService = authorizationManagerService;
        }
        public async Task<IEnumerable<ClientWorkoutPlan>> GetClientPlans(string clientId)
        {
            if (!await _authorizationManagerService.IsUserClient(clientId))
                throw new UnauthorizedAccessException("Only clients can access workout plans");

            return await _context.ClientWorkoutPlans
                .Where(cwp => cwp.ClientId == clientId)
                .Include(cwp => cwp.WorkoutPlan)
                    .ThenInclude(wp => wp.WorkoutPlanType)
                .Include(cwp => cwp.WorkoutPlan)
                    .ThenInclude(wp => wp.WorkoutPlanWorkouts)
                        .ThenInclude(wpw => wpw.Workout)
                            .ThenInclude(w => w.WorkoutExercises)
                                .ThenInclude(we => we.Exercise)
                .OrderByDescending(cwp => cwp.AssignedDate)
                .ToListAsync();
        }

        public async Task<ClientWorkoutPlan> GetClientPlanById(int planId, string clientId)
        {
            if (!await _authorizationManagerService.IsUserClient(clientId))
                throw new UnauthorizedAccessException("Only clients can access workout plans");

            var clientPlan = await _context.ClientWorkoutPlans
                .Include(cwp => cwp.WorkoutPlan)
                    .ThenInclude(wp => wp.WorkoutPlanType)
                .Include(cwp => cwp.WorkoutPlan)
                    .ThenInclude(wp => wp.WorkoutPlanWorkouts)
                        .ThenInclude(wpw => wpw.Workout)
                            .ThenInclude(w => w.WorkoutExercises)
                                .ThenInclude(we => we.Exercise)
                .FirstOrDefaultAsync(cwp => cwp.Id == planId && cwp.ClientId == clientId);

            if (clientPlan == null)
                throw new KeyNotFoundException($"Plan with ID {planId} not found");

            return clientPlan;
        }

        public async Task<bool> HasAccessToPlan(string clientId, int planId)
        {
            if (!await _authorizationManagerService.IsUserClient(clientId))
                return false;

            return await _context.ClientWorkoutPlans
                .AnyAsync(cwp => cwp.ClientId == clientId && cwp.Id == planId);
        }

        public async Task LogWorkout(WorkoutLog workoutLog, List<SetLog> setLogs, string clientId)
        {
            if (!await _authorizationManagerService.IsUserClient(clientId))
                throw new UnauthorizedAccessException("Only clients can log workouts");

            if (!await HasAccessToPlan(clientId, workoutLog.ClientWorkoutPlanId))
                throw new UnauthorizedAccessException("Unauthorized to access this workout plan");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate and prepare workout log
                workoutLog.ClientId = clientId;
                workoutLog.Date = DateTime.UtcNow;

                foreach (var setLog in setLogs)
                {
                    setLog.ClientId = clientId;
                }
                workoutLog.SetLogs = setLogs;

                await _context.WorkoutLogs.AddAsync(workoutLog);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<WorkoutLog>> GetWorkoutHistory(int clientWorkoutPlanId, string clientId)
        {
            if (!await _authorizationManagerService.IsUserClient(clientId))
                throw new UnauthorizedAccessException("Only clients can view workout history");

            if (!await HasAccessToPlan(clientId, clientWorkoutPlanId))
                throw new UnauthorizedAccessException("Unauthorized to access this workout plan");

            return await _context.WorkoutLogs
                .Where(wl => wl.ClientWorkoutPlanId == clientWorkoutPlanId && wl.ClientId == clientId)
                .Include(wl => wl.Workout)
                .Include(wl => wl.SetLogs)
                    .ThenInclude(sl => sl.Exercise)
                .OrderByDescending(wl => wl.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<SetLog>> GetWorkoutDetails(int workoutLogId, string clientId)
        {
            if (!await _authorizationManagerService.IsUserClient(clientId))
                throw new UnauthorizedAccessException("Only clients can view workout details");

            return await _context.SetLogs
                .Where(sl => sl.WorkoutLogId == workoutLogId && sl.ClientId == clientId)
                .Include(sl => sl.Exercise)
                .OrderBy(sl => sl.Exercise.Name)
                .ThenBy(sl => sl.SetNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkoutLog>> GetRecentWorkouts(string clientId, int count = 5)
        {
            if (!await _authorizationManagerService.IsUserClient(clientId))
                throw new UnauthorizedAccessException("Only clients can view their workouts");

            return await _context.WorkoutLogs
                .Where(wl => wl.ClientId == clientId)
                .Include(wl => wl.Workout)
                .Include(wl => wl.SetLogs)
                .OrderByDescending(wl => wl.Date)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Dictionary<DateTime, decimal>> GetExerciseProgressHistory(int exerciseId, string clientId)
        {
            if (!await _authorizationManagerService.IsUserClient(clientId))
                throw new UnauthorizedAccessException("Only clients can view their progress");

            var setLogs = await _context.SetLogs
                .Where(sl => sl.ExerciseId == exerciseId && sl.ClientId == clientId)
                .Include(sl => sl.WorkoutLog)
                .OrderBy(sl => sl.WorkoutLog.Date)
                .ToListAsync();

            return setLogs
                .GroupBy(sl => sl.WorkoutLog.Date.Date)
                .ToDictionary(
                    g => g.Key,
                    g => g.Max(sl => sl.Weight) // Get max weight for each date
                );
        }
    }
}

