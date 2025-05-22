using hyperTROPHYbuddy.Models;
using System.Numerics;

namespace hyperTROPHYbuddy.Services
{
    public interface IClientService
    {
       // Plan Management
        Task<IEnumerable<ClientWorkoutPlan>> GetClientPlans(string clientId);
        Task<ClientWorkoutPlan> GetClientPlanById(int planId, string clientId);
        Task<bool> HasAccessToPlan(string clientId, int planId);

        // Workout Logging
        Task LogWorkout(WorkoutLog workoutLog, List<SetLog> setLogs, string clientId);
        Task<IEnumerable<WorkoutLog>> GetWorkoutHistory(int clientWorkoutPlanId, string clientId);
        Task<IEnumerable<SetLog>> GetWorkoutDetails(int workoutLogId, string clientId);

        // Progress Tracking  
        Task<IEnumerable<WorkoutLog>> GetRecentWorkouts(string clientId, int count = 5);
        Task<Dictionary<DateTime, decimal>> GetExerciseProgressHistory(int exerciseId, string clientId);
    }
}
