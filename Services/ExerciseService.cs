using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly ApplicationDbContext _context;

        public ExerciseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Exercise>> GetAllAsync()
        {
            return await _context.Exercises
                .Include(e => e.CreatedBy)
                .ToListAsync();
        }

        public async Task<Exercise> GetByIdAsync(int id)
        {
            return await _context.Exercises
                .Include(e => e.CreatedBy)
                .FirstOrDefaultAsync(e => e.ExerciseId == id);
        }

        public async Task CreateAsync(Exercise exercise)
        {
            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Exercise exercise)
        {
            _context.Exercises.Update(exercise);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise != null)
            {
                _context.Exercises.Remove(exercise);
                await _context.SaveChangesAsync();
            }
        }
    }
}