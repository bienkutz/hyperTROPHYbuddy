using hyperTROPHYbuddy.Models;

namespace hyperTROPHYbuddy.Services
{
    public interface IWorkoutService
    {
        Task<IEnumerable<Workout>> GetWorkoutsByAdmin(string adminId);
        Task<Workout> GetWorkoutById(int id, string adminId);
        Task CreateWorkout(Workout workout, List<int> exerciseIds);
        Task UpdateWorkout(Workout workout, List<int> exerciseIds);
        Task DeleteWorkout(int id, string adminId);
        Task<bool> WorkoutExists(int id, string adminId);
    }
}
