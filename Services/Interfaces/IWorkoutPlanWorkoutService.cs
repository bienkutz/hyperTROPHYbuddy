using hyperTROPHYbuddy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Services.Interfaces
{
    public interface IWorkoutPlanWorkoutService
    {
        Task<IEnumerable<WorkoutPlanWorkout>> GetByPlanIdAsync(int workoutPlanId);
        Task<IEnumerable<WorkoutPlanWorkout>> GetByWorkoutIdAsync(int workoutId);
        Task<WorkoutPlanWorkout?> GetAsync(int workoutPlanId, int workoutId);
        Task CreateAsync(WorkoutPlanWorkout planWorkout);
        Task UpdateAsync(WorkoutPlanWorkout planWorkout);
        Task DeleteAsync(int workoutPlanId, int workoutId);
        Task DeleteAllForPlanAsync(int workoutPlanId);
    }
}