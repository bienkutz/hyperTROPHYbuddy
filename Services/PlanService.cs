using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.EntityFrameworkCore;

namespace hyperTROPHYbuddy.Services
{
    public class PlanService : IPlanService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWorkoutService _workoutService;

        public PlanService(ApplicationDbContext context, IWorkoutService workoutService)
        {
            _context = context;
            _workoutService = workoutService;
        }

        public async Task<IEnumerable<WorkoutPlan>> GetPlansByAdmin(string adminId)
        {
            return await _context.WorkoutPlans
                .Where(p => p.AdminId == adminId)
                .Include(p => p.WorkoutPlanType)
                .Include(p => p.WorkoutPlanWorkouts)
                .ThenInclude(wpw => wpw.Workout)
                .ToListAsync();
        }

        public async Task<WorkoutPlan> GetPlanById(int id, string adminId)
        {
            return await _context.WorkoutPlans
                .Include(p => p.WorkoutPlanType)
                .Include(p => p.WorkoutPlanWorkouts)
                .ThenInclude(wpw => wpw.Workout)
                .ThenInclude(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .FirstOrDefaultAsync(p => p.Id == id && p.AdminId == adminId);
        }

        public async Task CreatePlan(WorkoutPlan plan, List<int> workoutIds)
        {
            // Validate workout count (max 7)
            if (workoutIds.Count > 7)
            {
                throw new InvalidOperationException("A plan can have a maximum of 7 workouts.");
            }

            // Verify all workouts belong to the admin
            foreach (var workoutId in workoutIds)
            {
                if (!await _workoutService.WorkoutExists(workoutId, plan.AdminId))
                {
                    throw new InvalidOperationException($"Workout with ID {workoutId} not found or doesn't belong to you.");
                }
            }

            _context.WorkoutPlans.Add(plan);
            await _context.SaveChangesAsync();

            // Add plan workouts
            foreach (var workoutId in workoutIds)
            {
                _context.WorkoutPlanWorkouts.Add(new WorkoutPlanWorkout
                {
                    WorkoutPlanId = plan.Id,
                    WorkoutId = workoutId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePlan(WorkoutPlan plan, List<int> workoutIds)
        {
            // Validate workout count (max 7)
            if (workoutIds.Count > 7)
            {
                throw new InvalidOperationException("A plan can have a maximum of 7 workouts.");
            }

            // Verify all workouts belong to the admin
            foreach (var workoutId in workoutIds)
            {
                if (!await _workoutService.WorkoutExists(workoutId, plan.AdminId))
                {
                    throw new InvalidOperationException($"Workout with ID {workoutId} not found or doesn't belong to you.");
                }
            }

            // Remove existing plan workouts
            var existingWorkouts = await _context.WorkoutPlanWorkouts
                .Where(wpw => wpw.WorkoutPlanId == plan.Id)
                .ToListAsync();

            _context.WorkoutPlanWorkouts.RemoveRange(existingWorkouts);

            // Update plan
            _context.Entry(plan).State = EntityState.Modified;

            // Add new plan workouts
            foreach (var workoutId in workoutIds)
            {
                _context.WorkoutPlanWorkouts.Add(new WorkoutPlanWorkout
                {
                    WorkoutPlanId = plan.Id,
                    WorkoutId = workoutId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeletePlan(int id, string adminId)
        {
            var plan = await GetPlanById(id, adminId);
            if (plan != null)
            {
                _context.WorkoutPlans.Remove(plan);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> PlanExists(int id, string adminId)
        {
            return await _context.WorkoutPlans
                .AnyAsync(p => p.Id == id && p.AdminId == adminId);
        }

        public async Task<IEnumerable<WorkoutPlanType>> GetPlanTypes()
        {
            return await _context.WorkoutPlanTypes.ToListAsync();
        }

        public async Task AssignPlanToUser(int planId, string userId, string adminId)
        {
            // Verify the plan belongs to the admin
            var plan = await GetPlanById(planId, adminId);
            if (plan == null)
            {
                throw new InvalidOperationException("Plan not found or doesn't belong to you.");
            }

            // Check if the plan is already assigned to this user
            var existingAssignment = await _context.UserWorkoutPlans
                .FirstOrDefaultAsync(uwp => uwp.UserId == userId && uwp.WorkoutPlanId == planId);

            if (existingAssignment != null)
            {
                throw new InvalidOperationException("This plan is already assigned to the user.");
            }

            // Assign the plan
            var userWorkoutPlan = new UserWorkoutPlan
            {
                UserId = userId,
                WorkoutPlanId = planId,
                AssignedDate = DateTime.UtcNow
            };

            _context.UserWorkoutPlans.Add(userWorkoutPlan);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserWorkoutPlan>> GetAssignedPlans(string adminId)
        {
            return await _context.UserWorkoutPlans
                .Include(uwp => uwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanType)
                //.Include(uwp => uwp.User)
                .Where(uwp => uwp.WorkoutPlan.AdminId == adminId)
                .ToListAsync();
        }
    }
}
