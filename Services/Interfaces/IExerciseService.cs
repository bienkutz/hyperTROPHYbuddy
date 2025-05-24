using hyperTROPHYbuddy.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Services.Interfaces
{
    public interface IExerciseService
    {
        Task<IEnumerable<Exercise>> GetAllAsync();
        Task<Exercise> GetByIdAsync(int id);
        Task CreateAsync(Exercise exercise);
        Task UpdateAsync(Exercise exercise);
        Task DeleteAsync(int id);
    }
}