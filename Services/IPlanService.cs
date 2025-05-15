using hyperTROPHYbuddy.Models;

namespace hyperTROPHYbuddy.Services
{
    public interface IPlanService
    {
        Task<IEnumerable<WorkoutPlan>> GetPlansByAdmin(string adminId);
        Task<WorkoutPlan> GetPlanById(int id, string adminId);
        Task CreatePlan(WorkoutPlan plan, List<int> workoutIds);
        Task UpdatePlan(WorkoutPlan plan, List<int> workoutIds);
        Task DeletePlan(int id, string adminId);
        Task<bool> PlanExists(int id, string adminId);
        Task<IEnumerable<WorkoutPlanType>> GetPlanTypes();
        Task AssignPlanToUser(int planId, string userId, string adminId);
        Task<IEnumerable<UserWorkoutPlan>> GetAssignedPlans(string adminId);
    }
}
