using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace hyperTROPHYbuddy.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly IExerciseService _exerciseService;

        public WorkoutService(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            IExerciseService exerciseService)
        {
            _context = context;
            _authorizationService = authorizationService;
            _exerciseService = exerciseService;
        }

        public async Task<bool> IsUserAdmin(string userId) =>
            await _authorizationService.IsUserAdmin(userId);

        public async Task<IEnumerable<Workout>> GetWorkoutsByAdmin(string adminId)
        {
            if (!await IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can view workouts");

            return await _context.Workouts
                .Where(w => w.AdminId == adminId)
                .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .Include(w => w.Admin)
                .ToListAsync();
        }

        public async Task<Workout> GetWorkoutById(int id, string adminId)
        {
            if (!await IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can view workouts");

            return await _context.Workouts
                .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .Include(w => w.Admin)
                .FirstOrDefaultAsync(w => w.Id == id && w.AdminId == adminId);
        }

        public async Task CreateWorkout(Workout workout, List<int> exerciseIds)
        {
            if (!await IsUserAdmin(workout.AdminId))
                throw new UnauthorizedAccessException("Only admins can create workouts");

            if (exerciseIds.Count > 10)
                throw new InvalidOperationException("A workout can have a maximum of 10 exercises.");

            foreach (var exerciseId in exerciseIds)
            {
                if (!await _exerciseService.ExerciseExists(exerciseId, workout.AdminId))
                    throw new InvalidOperationException($"Exercise with ID {exerciseId} not found or doesn't belong to you.");
            }

            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            foreach (var exerciseId in exerciseIds)
            {
                _context.WorkoutExercises.Add(new WorkoutExercise
                {
                    WorkoutId = workout.Id,
                    ExerciseId = exerciseId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateWorkout(Workout workout, List<int> exerciseIds)
        {
            if (!await _authorizationService.CanModifyWorkout(workout.AdminId, workout.Id))
                throw new UnauthorizedAccessException("Unauthorized to modify this workout");

            if (exerciseIds.Count > 10)
                throw new InvalidOperationException("A workout can have a maximum of 10 exercises.");

            foreach (var exerciseId in exerciseIds)
            {
                if (!await _exerciseService.ExerciseExists(exerciseId, workout.AdminId))
                    throw new InvalidOperationException($"Exercise with ID {exerciseId} not found or doesn't belong to you.");
            }

            var existingExercises = await _context.WorkoutExercises
                .Where(we => we.WorkoutId == workout.Id)
                .ToListAsync();

            _context.WorkoutExercises.RemoveRange(existingExercises);
            _context.Entry(workout).State = EntityState.Modified;

            foreach (var exerciseId in exerciseIds)
            {
                _context.WorkoutExercises.Add(new WorkoutExercise
                {
                    WorkoutId = workout.Id,
                    ExerciseId = exerciseId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteWorkout(int id, string adminId)
        {
            if (!await IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can delete workouts");

            var workout = await GetWorkoutById(id, adminId);
            if (workout != null)
            {
                _context.Workouts.Remove(workout);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> WorkoutExists(int id, string adminId)
        {
            if (!await IsUserAdmin(adminId))
                return false;

            return await _context.Workouts
                .AnyAsync(w => w.Id == id && w.AdminId == adminId);
        }
    }
}
