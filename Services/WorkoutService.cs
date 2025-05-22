using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.EntityFrameworkCore;

namespace hyperTROPHYbuddy.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationManagerService _authorizationManagerService;
        private readonly IExerciseService _exerciseService;

        public WorkoutService(
            ApplicationDbContext context,
            IAuthorizationManagerService authorizationManagerService,
            IExerciseService exerciseService)
        {
            _context = context;
            _authorizationManagerService = authorizationManagerService;
            _exerciseService = exerciseService;
        }

        public async Task<IEnumerable<Workout>> GetWorkoutsByAdmin(string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can view workouts");

            return await _context.Workouts
                .Where(w => w.AdminId == adminId)
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<Workout> GetWorkoutById(int id, string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can view workouts");

            var workout = await _context.Workouts
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                .FirstOrDefaultAsync(w => w.Id == id && w.AdminId == adminId);

            if (workout == null)
                throw new KeyNotFoundException($"Workout with ID {id} not found");

            return workout;
        }

        public async Task<Workout> CreateWorkout(Workout workout, List<int> exerciseIds, string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can create workouts");

            if (string.IsNullOrWhiteSpace(workout.Name))
                throw new ArgumentException("Workout name is required");

            if (exerciseIds == null || !exerciseIds.Any())
                throw new ArgumentException("At least one exercise must be selected");

            if (exerciseIds.Count > 10)
                throw new InvalidOperationException("A workout can have a maximum of 10 exercises");

            // Validate all exercises belong to admin
            foreach (var exerciseId in exerciseIds)
            {
                if (!await _exerciseService.ExerciseExists(exerciseId, adminId))
                    throw new InvalidOperationException($"Exercise with ID {exerciseId} not found or doesn't belong to you");
            }

            workout.AdminId = adminId;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Workouts.Add(workout);
                await _context.SaveChangesAsync();

                var workoutExercises = exerciseIds.Select(exerciseId => new WorkoutExercise
                {
                    WorkoutId = workout.Id,
                    ExerciseId = exerciseId
                });

                await _context.WorkoutExercises.AddRangeAsync(workoutExercises);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return workout;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateWorkout(Workout workout, List<int> exerciseIds, string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can modify workouts");

            if (string.IsNullOrWhiteSpace(workout.Name))
                throw new ArgumentException("Workout name is required");

            if (exerciseIds == null || !exerciseIds.Any())
                throw new ArgumentException("At least one exercise must be selected");

            if (exerciseIds.Count > 10)
                throw new InvalidOperationException("A workout can have a maximum of 10 exercises");

            var existingWorkout = await _context.Workouts
                .Include(w => w.WorkoutExercises)
                .FirstOrDefaultAsync(w => w.Id == workout.Id && w.AdminId == adminId);

            if (existingWorkout == null)
                throw new KeyNotFoundException($"Workout with ID {workout.Id} not found");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Update basic properties
                existingWorkout.Name = workout.Name;

                // Remove existing exercise relationships
                _context.WorkoutExercises.RemoveRange(existingWorkout.WorkoutExercises);

                // Add new exercise relationships
                var workoutExercises = exerciseIds.Select(exerciseId => new WorkoutExercise
                {
                    WorkoutId = workout.Id,
                    ExerciseId = exerciseId
                });

                await _context.WorkoutExercises.AddRangeAsync(workoutExercises);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteWorkout(int id, string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can delete workouts");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var workout = await _context.Workouts
                    .Include(w => w.WorkoutExercises)
                    .Include(w => w.WorkoutPlanWorkouts)
                    .FirstOrDefaultAsync(w => w.Id == id && w.AdminId == adminId);

                if (workout == null)
                    throw new KeyNotFoundException($"Workout with ID {id} not found");

                if (workout.WorkoutPlanWorkouts.Any())
                    throw new InvalidOperationException("Cannot delete workout that is used in workout plans");

                _context.WorkoutExercises.RemoveRange(workout.WorkoutExercises);
                _context.Workouts.Remove(workout);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> WorkoutExists(int id, string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                return false;

            return await _context.Workouts
                .AnyAsync(w => w.Id == id && w.AdminId == adminId);
        }

        public async Task<IEnumerable<Workout>> SearchWorkouts(string searchTerm, string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can search workouts");

            return await _context.Workouts
                .Where(w => w.AdminId == adminId &&
                           (string.IsNullOrEmpty(searchTerm) ||
                            w.Name.Contains(searchTerm)))
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }
    }
}
