using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Workout>> GetAllAsync()
        {
            return await _context.Workouts
                .Include(w => w.CreatedBy)
                .ToListAsync();
        }

        public async Task<Workout?> GetByIdAsync(int id)
        {
            return await _context.Workouts
                .Include(w => w.CreatedBy)
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                .FirstOrDefaultAsync(w => w.WorkoutId == id);
        }

        public async Task CreateAsync(Workout workout)
        {
            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Workout workout)
        {
            _context.Workouts.Update(workout);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout != null)
            {
                _context.Workouts.Remove(workout);
                await _context.SaveChangesAsync();
            }
        }
    }
}