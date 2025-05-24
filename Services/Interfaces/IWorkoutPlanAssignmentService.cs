using hyperTROPHYbuddy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Services.Interfaces
{
    public interface IWorkoutPlanAssignmentService
    {
        Task<IEnumerable<WorkoutPlanAssignment>> GetAllAsync();
        Task<WorkoutPlanAssignment?> GetByIdAsync(int id);
        Task<IEnumerable<WorkoutPlanAssignment>> GetByClientIdAsync(string clientId);
        Task<IEnumerable<WorkoutPlanAssignment>> GetByAdminIdAsync(string adminId);
        Task CreateAsync(WorkoutPlanAssignment assignment);
        Task UpdateAsync(WorkoutPlanAssignment assignment);
        Task DeleteAsync(int id);
    }
}