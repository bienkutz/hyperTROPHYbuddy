using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace hyperTROPHYbuddy.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;

        public ClientService(
            ApplicationDbContext context,
            IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }
        public async Task<bool> IsValidClient(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            // Using AuthorizationService to check if the user exists and has the Client role
            try
            {
                var clientPlan = await _context.ClientWorkoutPlans
                    .FirstOrDefaultAsync(cwp => cwp.ClientId == userId);

                return clientPlan != null;
            }
            catch
            {
                return false;
            }
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
            if (!await _authorizationService.CanAccessClientPlan(clientId, clientWorkoutPlanId))
                throw new UnauthorizedAccessException("Unauthorized to access this workout plan");

            var workoutLog = new WorkoutLog
            {
                ClientWorkoutPlanId = clientWorkoutPlanId,
                WorkoutId = workoutId,
                ClientId = clientId,
                Date = date,
                SetLogs = setLogs
            };

            foreach (var log in setLogs)
            {
                log.ClientId = clientId;
            }

            _context.WorkoutLogs.Add(workoutLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<WorkoutLog>> GetWorkoutHistory(int clientWorkoutPlanId, string clientId)
        {
            if (!await _authorizationService.CanAccessClientPlan(clientId, clientWorkoutPlanId))
                throw new UnauthorizedAccessException("Unauthorized to access this workout plan");

            return await _context.WorkoutLogs
                .Where(wl => wl.ClientWorkoutPlanId == clientWorkoutPlanId && wl.ClientId == clientId)
                .Include(wl => wl.Workout)
                .Include(wl => wl.SetLogs)
                    .ThenInclude(sl => sl.Exercise)
                .OrderByDescending(wl => wl.Date)
                .ToListAsync();
        }
    }
}
