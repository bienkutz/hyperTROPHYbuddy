using System.Collections.Generic;
using System.Threading.Tasks;
using hyperTROPHYbuddy.Models;

namespace hyperTROPHYbuddy.Services.Interfaces
{
    public interface IWorkoutExerciseService
    {
        Task<IEnumerable<WorkoutExercise>> GetByWorkoutIdAsync(int workoutId);
        Task<IEnumerable<WorkoutExercise>> GetByExerciseIdAsync(int exerciseId);
        Task AddAsync(WorkoutExercise workoutExercise);
        Task RemoveAsync(int workoutId, int exerciseId);
        Task RemoveAllForWorkoutAsync(int workoutId);
    }
}