using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.EntityFrameworkCore;

namespace hyperTROPHYbuddy.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationManagerService _authorizationManagerService;

        public ExerciseService(
            ApplicationDbContext context,
            IAuthorizationManagerService authorizationManagerService)
        {
            _context = context;
            _authorizationManagerService = authorizationManagerService;
        }

        public async Task<IEnumerable<Exercise>> GetExercisesByAdmin(string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can view exercises");

            return await _context.Exercises
                .Where(e => e.AdminId == adminId)
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<Exercise> GetExerciseById(int id, string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can view exercises");

            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == id && e.AdminId == adminId);

            if (exercise == null)
                throw new KeyNotFoundException($"Exercise with ID {id} not found");

            return exercise;
        }

        public async Task<Exercise> CreateExercise(Exercise exercise, string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can create exercises");

            if (string.IsNullOrWhiteSpace(exercise.Name))
                throw new ArgumentException("Exercise name is required");

            exercise.AdminId = adminId;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Exercises.Add(exercise);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return exercise;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateExercise(Exercise exercise, string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can modify exercises");

            if (string.IsNullOrWhiteSpace(exercise.Name))
                throw new ArgumentException("Exercise name is required");

            var existingExercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == exercise.Id && e.AdminId == adminId);

            if (existingExercise == null)
                throw new KeyNotFoundException($"Exercise with ID {exercise.Id} not found");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                existingExercise.Name = exercise.Name;
                existingExercise.Description = exercise.Description;
                existingExercise.VideoLink = exercise.VideoLink;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteExercise(int id, string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can delete exercises");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var exercise = await _context.Exercises
                    .Include(e => e.WorkoutExercises)
                    .FirstOrDefaultAsync(e => e.Id == id && e.AdminId == adminId);

                if (exercise == null)
                    throw new KeyNotFoundException($"Exercise with ID {id} not found");

                if (exercise.WorkoutExercises.Any())
                    throw new InvalidOperationException("Cannot delete exercise that is used in workouts");

                _context.Exercises.Remove(exercise);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ExerciseExists(int id, string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                return false;

            return await _context.Exercises
                .AnyAsync(e => e.Id == id && e.AdminId == adminId);
        }

        public async Task<IEnumerable<Exercise>> SearchExercises(string searchTerm, string adminId)
        {
            if (!await _authorizationManagerService.IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can search exercises");

            return await _context.Exercises
                .Where(e => e.AdminId == adminId &&
                           (string.IsNullOrEmpty(searchTerm) ||
                            e.Name.Contains(searchTerm) ||
                            e.Description.Contains(searchTerm)))
                .OrderBy(e => e.Name)
                .ToListAsync();
        }
    }
}
