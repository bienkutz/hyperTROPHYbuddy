using hyperTROPHYbuddy.Models;

namespace hyperTROPHYbuddy.Services
{
    public interface IExerciseService
    {
        Task<IEnumerable<Exercise>> GetExercisesByAdmin(string adminId);
        Task<Exercise> GetExerciseById(int id, string adminId);
        Task<Exercise> CreateExercise(Exercise exercise, string adminId);
        Task UpdateExercise(Exercise exercise, string adminId);
        Task DeleteExercise(int id, string adminId);
        Task<bool> ExerciseExists(int id, string adminId);
        Task<IEnumerable<Exercise>> SearchExercises(string searchTerm, string adminId);
    }
}
