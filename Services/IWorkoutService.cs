using hyperTROPHYbuddy.Models;

namespace hyperTROPHYbuddy.Services
{
    public interface IWorkoutService
    {
        Task<IEnumerable<Workout>> GetWorkoutsByAdmin(string adminId);
        Task<Workout> GetWorkoutById(int id, string adminId);
        Task<Workout> CreateWorkout(Workout workout, List<int> exerciseIds, string adminId);
        Task UpdateWorkout(Workout workout, List<int> exerciseIds, string adminId);
        Task DeleteWorkout(int id, string adminId);
        Task<bool> WorkoutExists(int id, string adminId);
        Task<IEnumerable<Workout>> SearchWorkouts(string searchTerm, string adminId);
    }
}

