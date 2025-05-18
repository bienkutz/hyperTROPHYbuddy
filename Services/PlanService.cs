using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
using Microsoft.EntityFrameworkCore;

public class PlanService : IPlanService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly IWorkoutService _workoutService;

    public PlanService(
        ApplicationDbContext context,
        IAuthorizationService authorizationService,
        IWorkoutService workoutService)
    {
        _context = context;
        _authorizationService = authorizationService;
        _workoutService = workoutService;
    }

    public async Task<bool> IsUserAdmin(string userId) =>
        await _authorizationService.IsUserAdmin(userId);

    public async Task<IEnumerable<WorkoutPlan>> GetPlansByAdmin(string adminId)
    {
        if (!await IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can view plans");

        return await _context.WorkoutPlans
            .Where(p => p.AdminId == adminId)
            .Include(p => p.WorkoutPlanType)
            .Include(p => p.WorkoutPlanWorkouts)
                .ThenInclude(wpw => wpw.Workout)
            .Include(p => p.Admin)
            .ToListAsync();
    }

    public async Task<WorkoutPlan> GetPlanById(int id, string adminId)
    {
        if (!await IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can view this plan");

        return await _context.WorkoutPlans
            .Include(p => p.WorkoutPlanType)
            .Include(p => p.WorkoutPlanWorkouts)
                .ThenInclude(wpw => wpw.Workout)
                    .ThenInclude(w => w.WorkoutExercises)
                        .ThenInclude(we => we.Exercise)
            .Include(p => p.Admin)
            .FirstOrDefaultAsync(p => p.Id == id && p.AdminId == adminId);
    }

    public async Task CreatePlan(WorkoutPlan plan, List<int> workoutIds)
    {
        if (!await IsUserAdmin(plan.AdminId))
            throw new UnauthorizedAccessException("Only admins can create plans");

        if (workoutIds.Count > 7)
            throw new InvalidOperationException("A plan can have a maximum of 7 workouts.");

        foreach (var workoutId in workoutIds)
        {
            if (!await _workoutService.WorkoutExists(workoutId, plan.AdminId))
                throw new InvalidOperationException($"Workout with ID {workoutId} not found or doesn't belong to you.");
        }

        _context.WorkoutPlans.Add(plan);
        await _context.SaveChangesAsync();

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
        if (!await _authorizationService.CanModifyPlan(plan.AdminId, plan.Id))
            throw new UnauthorizedAccessException("Unauthorized to modify this plan");

        if (workoutIds.Count > 7)
            throw new InvalidOperationException("A plan can have a maximum of 7 workouts.");

        foreach (var workoutId in workoutIds)
        {
            if (!await _workoutService.WorkoutExists(workoutId, plan.AdminId))
                throw new InvalidOperationException($"Workout with ID {workoutId} not found or doesn't belong to you.");
        }

        var existingWorkouts = await _context.WorkoutPlanWorkouts
            .Where(wpw => wpw.WorkoutPlanId == plan.Id)
            .ToListAsync();

        _context.WorkoutPlanWorkouts.RemoveRange(existingWorkouts);
        _context.Entry(plan).State = EntityState.Modified;

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
        if (!await IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can delete plans");

        var plan = await GetPlanById(id, adminId);
        if (plan != null)
        {
            _context.WorkoutPlans.Remove(plan);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> PlanExists(int id, string adminId)
    {
        if (!await IsUserAdmin(adminId))
            return false;

        return await _context.WorkoutPlans
            .AnyAsync(p => p.Id == id && p.AdminId == adminId);
    }

    public async Task<IEnumerable<WorkoutPlanType>> GetPlanTypes()
    {
        return await _context.WorkoutPlanTypes.ToListAsync();
    }

    public async Task AssignPlanToClient(int planId, string clientId, string adminId)
    {
        if (!await IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can assign plans");

        var plan = await GetPlanById(planId, adminId);
        if (plan == null)
            throw new InvalidOperationException("Plan not found or doesn't belong to you.");

        var existingAssignment = await _context.ClientWorkoutPlans
            .FirstOrDefaultAsync(cwp => cwp.ClientId == clientId && cwp.WorkoutPlanId == planId);

        if (existingAssignment != null)
            throw new InvalidOperationException("This plan is already assigned to the client.");

        var clientWorkoutPlan = new ClientWorkoutPlan
        {
            ClientId = clientId,
            WorkoutPlanId = planId,
            AssignedDate = DateTime.UtcNow
        };

        _context.ClientWorkoutPlans.Add(clientWorkoutPlan);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ClientWorkoutPlan>> GetAssignedPlans(string adminId)
    {
        if (!await IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can view assigned plans");

        return await _context.ClientWorkoutPlans
            .Include(cwp => cwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanType)
            .Include(cwp => cwp.Client)
            .Where(cwp => cwp.WorkoutPlan.AdminId == adminId)
            .ToListAsync();
    }
}