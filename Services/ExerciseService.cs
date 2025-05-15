using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.EntityFrameworkCore;

namespace hyperTROPHYbuddy.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly ApplicationDbContext _context;

        public ExerciseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Exercise>> GetExercisesByAdmin(string adminId)
        {
            return await _context.Exercises
                .Where(e => e.AdminId == adminId)
                .ToListAsync();
        }

        public async Task<Exercise> GetExerciseById(int id, string adminId)
        {
            return await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == id && e.AdminId == adminId);
        }

        public async Task CreateExercise(Exercise exercise)
        {
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateExercise(Exercise exercise)
        {
            _context.Entry(exercise).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExercise(int id, string adminId)
        {
            var exercise = await GetExerciseById(id, adminId);
            if (exercise != null)
            {
                _context.Exercises.Remove(exercise);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExerciseExists(int id, string adminId)
        {
            return await _context.Exercises
                .AnyAsync(e => e.Id == id && e.AdminId == adminId);
        }
    }
}
