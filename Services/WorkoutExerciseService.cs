using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hyperTROPHYbuddy.Services
{
    public class WorkoutExerciseService : IWorkoutExerciseService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutExerciseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkoutExercise>> GetByWorkoutIdAsync(int workoutId)
        {
            return await _context.Set<WorkoutExercise>()
                .Include(we => we.Exercise)
                .Where(we => we.WorkoutId == workoutId)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkoutExercise>> GetByExerciseIdAsync(int exerciseId)
        {
            return await _context.Set<WorkoutExercise>()
                .Include(we => we.Workout)
                .Where(we => we.ExerciseId == exerciseId)
                .ToListAsync();
        }

        public async Task AddAsync(WorkoutExercise workoutExercise)
        {
            _context.Set<WorkoutExercise>().Add(workoutExercise);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int workoutId, int exerciseId)
        {
            var entity = await _context.Set<WorkoutExercise>()
                .FirstOrDefaultAsync(we => we.WorkoutId == workoutId && we.ExerciseId == exerciseId);
            if (entity != null)
            {
                _context.Set<WorkoutExercise>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAllForWorkoutAsync(int workoutId)
        {
            var entities = _context.Set<WorkoutExercise>().Where(we => we.WorkoutId == workoutId);
            _context.Set<WorkoutExercise>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}