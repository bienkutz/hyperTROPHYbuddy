using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hyperTROPHYbuddy.Services
{
    public class WorkoutPlanService : IWorkoutPlanService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutPlanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkoutPlan>> GetAllAsync()
        {
            return await _context.WorkoutPlans
                .Include(wp => wp.WorkoutPlanWorkouts)
                .ThenInclude(wpw => wpw.Workout)
                .ToListAsync();
        }

        public async Task<WorkoutPlan?> GetByIdAsync(int id)
        {
            return await _context.WorkoutPlans
                .Include(wp => wp.WorkoutPlanWorkouts)
                    .ThenInclude(wpw => wpw.Workout)
                        .ThenInclude(w => w.WorkoutExercises)
                            .ThenInclude(we => we.Exercise)
                .FirstOrDefaultAsync(wp => wp.WorkoutPlanId == id);
        }

        public async Task CreateAsync(WorkoutPlan workoutPlan)
        {
            _context.WorkoutPlans.Add(workoutPlan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkoutPlan workoutPlan)
        {
            _context.WorkoutPlans.Update(workoutPlan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var plan = await _context.WorkoutPlans.FindAsync(id);
            if (plan != null)
            {
                _context.WorkoutPlans.Remove(plan);
                await _context.SaveChangesAsync();
            }
        }
    }
}