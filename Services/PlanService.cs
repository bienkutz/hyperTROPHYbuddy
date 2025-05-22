using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services;
using Microsoft.EntityFrameworkCore;

public class PlanService : IPlanService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthorizationManagerService _authorizationManagerService;
    private readonly IWorkoutService _workoutService;

    public PlanService(
        ApplicationDbContext context,
        IAuthorizationManagerService authorizationManagerService,
        IWorkoutService workoutService)
    {
        _context = context;
        _authorizationManagerService = authorizationManagerService;
        _workoutService = workoutService;
    }

    public async Task<IEnumerable<WorkoutPlan>> GetPlansByAdmin(string adminId)
    {
        if (!await _authorizationManagerService.IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can view plans");

        return await _context.WorkoutPlans
            .Where(p => p.AdminId == adminId)
            .Include(p => p.WorkoutPlanType)
            .Include(p => p.WorkoutPlanWorkouts)
                .ThenInclude(wpw => wpw.Workout)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<WorkoutPlan> GetPlanById(int id, string adminId)
    {
        if (!await _authorizationManagerService.IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can view this plan");

        var plan = await _context.WorkoutPlans
            .Include(p => p.WorkoutPlanType)
            .Include(p => p.WorkoutPlanWorkouts)
                .ThenInclude(wpw => wpw.Workout)
                    .ThenInclude(w => w.WorkoutExercises)
                        .ThenInclude(we => we.Exercise)
            .FirstOrDefaultAsync(p => p.Id == id && p.AdminId == adminId);

        if (plan == null)
            throw new KeyNotFoundException($"Plan with ID {id} not found");

        return plan;
    }

    public async Task<WorkoutPlan> CreatePlan(WorkoutPlan plan, List<int> workoutIds, string adminId)
    {
        if (!await _authorizationManagerService.IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can create plans");

        if (string.IsNullOrWhiteSpace(plan.Name))
            throw new ArgumentException("Plan name is required");

        if (workoutIds == null || !workoutIds.Any())
            throw new ArgumentException("At least one workout must be selected");

        if (workoutIds.Count > 7)
            throw new InvalidOperationException("A plan can have a maximum of 7 workouts");

        plan.AdminId = adminId;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Validate all workouts belong to admin
            foreach (var workoutId in workoutIds)
            {
                if (!await _workoutService.WorkoutExists(workoutId, adminId))
                    throw new InvalidOperationException($"Workout with ID {workoutId} not found or doesn't belong to you");
            }

            _context.WorkoutPlans.Add(plan);
            await _context.SaveChangesAsync();

            var planWorkouts = workoutIds.Select(workoutId => new WorkoutPlanWorkout
            {
                WorkoutPlanId = plan.Id,
                WorkoutId = workoutId
            });

            await _context.WorkoutPlanWorkouts.AddRangeAsync(planWorkouts);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return plan;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdatePlan(WorkoutPlan plan, List<int> workoutIds, string adminId)
    {
        if (!await _authorizationManagerService.IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can modify plans");

        if (string.IsNullOrWhiteSpace(plan.Name))
            throw new ArgumentException("Plan name is required");

        if (workoutIds == null || !workoutIds.Any())
            throw new ArgumentException("At least one workout must be selected");

        if (workoutIds.Count > 7)
            throw new InvalidOperationException("A plan can have a maximum of 7 workouts");

        var existingPlan = await _context.WorkoutPlans
            .Include(p => p.WorkoutPlanWorkouts)
            .FirstOrDefaultAsync(p => p.Id == plan.Id && p.AdminId == adminId);

        if (existingPlan == null)
            throw new KeyNotFoundException($"Plan with ID {plan.Id} not found");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Update basic properties
            existingPlan.Name = plan.Name;
            existingPlan.Description = plan.Description;
            existingPlan.WorkoutPlanTypeId = plan.WorkoutPlanTypeId;

            // Remove existing workout relationships
            _context.WorkoutPlanWorkouts.RemoveRange(existingPlan.WorkoutPlanWorkouts);

            // Add new workout relationships
            var planWorkouts = workoutIds.Select(workoutId => new WorkoutPlanWorkout
            {
                WorkoutPlanId = plan.Id,
                WorkoutId = workoutId
            });

            await _context.WorkoutPlanWorkouts.AddRangeAsync(planWorkouts);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeletePlan(int id, string adminId)
    {
        if (!await _authorizationManagerService.IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can delete plans");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var plan = await _context.WorkoutPlans
                .Include(p => p.WorkoutPlanWorkouts)
                .Include(p => p.ClientWorkoutPlans)
                    .ThenInclude(cwp => cwp.WorkoutLogs)
                        .ThenInclude(wl => wl.SetLogs)
                .FirstOrDefaultAsync(p => p.Id == id && p.AdminId == adminId);

            if (plan == null)
                throw new KeyNotFoundException($"Plan with ID {id} not found");

            // Remove all related logs
            foreach (var clientPlan in plan.ClientWorkoutPlans)
            {
                foreach (var workoutLog in clientPlan.WorkoutLogs)
                {
                    _context.SetLogs.RemoveRange(workoutLog.SetLogs);
                    _context.WorkoutLogs.Remove(workoutLog);
                }
                _context.ClientWorkoutPlans.Remove(clientPlan);
            }

            _context.WorkoutPlanWorkouts.RemoveRange(plan.WorkoutPlanWorkouts);
            _context.WorkoutPlans.Remove(plan);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<WorkoutPlanType>> GetPlanTypes()
    {
        return await _context.WorkoutPlanTypes
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task AssignPlanToClient(int planId, string clientId, string adminId)
    {
        if (!await _authorizationManagerService.IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can assign plans");

        if (string.IsNullOrEmpty(clientId))
            throw new ArgumentException("Client ID is required");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var plan = await GetPlanById(planId, adminId);

            var existingAssignment = await _context.ClientWorkoutPlans
                .AnyAsync(cwp => cwp.ClientId == clientId && cwp.WorkoutPlanId == planId);

            if (existingAssignment)
                throw new InvalidOperationException("This plan is already assigned to the client");

            var clientPlan = new ClientWorkoutPlan
            {
                ClientId = clientId,
                WorkoutPlanId = planId,
                AssignedDate = DateTime.UtcNow
            };

            await _context.ClientWorkoutPlans.AddAsync(clientPlan);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<ClientWorkoutPlan>> GetAssignedPlans(string adminId)
    {
        if (!await _authorizationManagerService.IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can view assigned plans");

        return await _context.ClientWorkoutPlans
            .Include(cwp => cwp.WorkoutPlan)
                .ThenInclude(wp => wp.WorkoutPlanType)
            .Include(cwp => cwp.Client)
            .Where(cwp => cwp.WorkoutPlan.AdminId == adminId)
            .OrderByDescending(cwp => cwp.AssignedDate)
            .ToListAsync();
    }

    public async Task<bool> PlanExists(int id, string adminId)
    {
        if (!await _authorizationManagerService.IsUserAdmin(adminId))
            return false;

        return await _context.WorkoutPlans
            .AnyAsync(p => p.Id == id && p.AdminId == adminId);
    }

    public async Task<IEnumerable<WorkoutPlan>> SearchPlans(string searchTerm, string adminId)
    {
        if (!await _authorizationManagerService.IsUserAdmin(adminId))
            throw new UnauthorizedAccessException("Only admins can search plans");

        return await _context.WorkoutPlans
            .Where(p => p.AdminId == adminId &&
                       (string.IsNullOrEmpty(searchTerm) ||
                        p.Name.Contains(searchTerm) ||
                        p.Description.Contains(searchTerm)))
            .Include(p => p.WorkoutPlanType)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

}