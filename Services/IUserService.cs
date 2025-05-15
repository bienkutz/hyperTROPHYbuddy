using hyperTROPHYbuddy.Models;

namespace hyperTROPHYbuddy.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserWorkoutPlan>> GetUserPlans(string userId);
        Task<UserWorkoutPlan> GetUserPlanById(int id, string userId);
        Task LogWorkout(int userWorkoutPlanId, int workoutId, string userId, DateTime date, List<SetLog> setLogs);
        Task<IEnumerable<WorkoutLog>> GetWorkoutHistory(int userWorkoutPlanId, string userId);
    }
}
