namespace hyperTROPHYbuddy.Services
{
    public interface IAuthorizationService
    {
        Task<bool> IsUserAdmin(string userId);
        Task<bool> CanAccessClientPlan(string userId, int planId);
        Task<bool> CanModifyExercise(string userId, int exerciseId);
        Task<bool> CanModifyWorkout(string userId, int workoutId);
        Task<bool> CanModifyPlan(string userId, int planId);
        Task<bool> IsValidClient(string userId);
    }
}
