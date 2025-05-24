using hyperTROPHYbuddy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Services.Interfaces
{
    public interface IWorkoutPlanService
    {
        Task<IEnumerable<WorkoutPlan>> GetAllAsync();
        Task<WorkoutPlan?> GetByIdAsync(int id);
        Task CreateAsync(WorkoutPlan workoutPlan);
        Task UpdateAsync(WorkoutPlan workoutPlan);
        Task DeleteAsync(int id);
    }
}