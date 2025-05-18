using hyperTROPHYbuddy.Models;

namespace hyperTROPHYbuddy.Services
{
    public interface IExerciseService
    {
        Task<bool> IsUserAdmin(string userId);  
        Task<IEnumerable<Exercise>> GetExercisesByAdmin(string adminId);
        Task<Exercise> GetExerciseById(int id, string adminId);
        Task CreateExercise(Exercise exercise);
        Task UpdateExercise(Exercise exercise);
        Task DeleteExercise(int id, string adminId);
        Task<bool> ExerciseExists(int id, string adminId);
    }
}
