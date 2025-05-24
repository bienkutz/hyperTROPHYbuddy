using hyperTROPHYbuddy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Services.Interfaces
{
    public interface IWorkoutService
    {
        Task<IEnumerable<Workout>> GetAllAsync();
        Task<Workout?> GetByIdAsync(int id);
        Task CreateAsync(Workout workout);
        Task UpdateAsync(Workout workout);
        Task DeleteAsync(int id);
    }
}