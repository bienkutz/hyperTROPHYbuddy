using hyperTROPHYbuddy.Models;

namespace hyperTROPHYbuddy.Services
{
    public interface IClientService
    {
        Task<IEnumerable<ClientWorkoutPlan>> GetClientPlans(string clientId);
        Task<ClientWorkoutPlan> GetClientPlanById(int id, string clientId);
        Task LogWorkout(int clientWorkoutPlanId, int workoutId, string clientId, DateTime date, List<SetLog> setLogs);
        Task<IEnumerable<WorkoutLog>> GetWorkoutHistory(int clientWorkoutPlanId, string clientId);
        Task<bool> IsValidClient(string userId);  
    }
}
