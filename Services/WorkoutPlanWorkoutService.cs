using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Services
{
    public class WorkoutPlanWorkoutService : IWorkoutPlanWorkoutService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutPlanWorkoutService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkoutPlanWorkout>> GetByPlanIdAsync(int workoutPlanId)
        {
            return await _context.Set<WorkoutPlanWorkout>()
                .Include(pw => pw.Workout)
                .Where(pw => pw.WorkoutPlanId == workoutPlanId)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkoutPlanWorkout>> GetByWorkoutIdAsync(int workoutId)
        {
            return await _context.Set<WorkoutPlanWorkout>()
                .Include(pw => pw.WorkoutPlan)
                .Where(pw => pw.WorkoutId == workoutId)
                .ToListAsync();
        }

        public async Task<WorkoutPlanWorkout?> GetAsync(int workoutPlanId, int workoutId)
        {
            return await _context.Set<WorkoutPlanWorkout>()
                .Include(pw => pw.Workout)
                .Include(pw => pw.WorkoutPlan)
                .FirstOrDefaultAsync(pw => pw.WorkoutPlanId == workoutPlanId && pw.WorkoutId == workoutId);
        }

        public async Task CreateAsync(WorkoutPlanWorkout planWorkout)
        {
            _context.Set<WorkoutPlanWorkout>().Add(planWorkout);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkoutPlanWorkout planWorkout)
        {
            _context.Set<WorkoutPlanWorkout>().Update(planWorkout);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int workoutPlanId, int workoutId)
        {
            var entity = await _context.Set<WorkoutPlanWorkout>()
                .FirstOrDefaultAsync(pw => pw.WorkoutPlanId == workoutPlanId && pw.WorkoutId == workoutId);
            if (entity != null)
            {
                _context.Set<WorkoutPlanWorkout>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllForPlanAsync(int workoutPlanId)
        {
            var entities = _context.Set<WorkoutPlanWorkout>().Where(pw => pw.WorkoutPlanId == workoutPlanId);
            _context.Set<WorkoutPlanWorkout>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}