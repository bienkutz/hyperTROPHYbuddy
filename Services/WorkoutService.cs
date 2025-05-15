using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.EntityFrameworkCore;

namespace hyperTROPHYbuddy.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExerciseService _exerciseService;

        public WorkoutService(ApplicationDbContext context, IExerciseService exerciseService)
        {
            _context = context;
            _exerciseService = exerciseService;
        }

        public async Task<IEnumerable<Workout>> GetWorkoutsByAdmin(string adminId)
        {
            return await _context.Workouts
                .Where(w => w.AdminId == adminId)
                .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .ToListAsync();
        }

        public async Task<Workout> GetWorkoutById(int id, string adminId)
        {
            return await _context.Workouts
                .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Exercise)
                .FirstOrDefaultAsync(w => w.Id == id && w.AdminId == adminId);
        }

        public async Task CreateWorkout(Workout workout, List<int> exerciseIds)
        {
            // Validate exercise count (max 10)
            if (exerciseIds.Count > 10)
            {
                throw new InvalidOperationException("A workout can have a maximum of 10 exercises.");
            }

            // Verify all exercises belong to the admin
            foreach (var exerciseId in exerciseIds)
            {
                if (!await _exerciseService.ExerciseExists(exerciseId, workout.AdminId))
                {
                    throw new InvalidOperationException($"Exercise with ID {exerciseId} not found or doesn't belong to you.");
                }
            }

            _context.Workouts.Add(workout);
            await _context.SaveChangesAsync();

            // Add workout exercises
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
            // Validate exercise count (max 10)
            if (exerciseIds.Count > 10)
            {
                throw new InvalidOperationException("A workout can have a maximum of 10 exercises.");
            }

            // Verify all exercises belong to the admin
            foreach (var exerciseId in exerciseIds)
            {
                if (!await _exerciseService.ExerciseExists(exerciseId, workout.AdminId))
                {
                    throw new InvalidOperationException($"Exercise with ID {exerciseId} not found or doesn't belong to you.");
                }
            }

            // Remove existing workout exercises
            var existingExercises = await _context.WorkoutExercises
                .Where(we => we.WorkoutId == workout.Id)
                .ToListAsync();

            _context.WorkoutExercises.RemoveRange(existingExercises);

            // Update workout
            _context.Entry(workout).State = EntityState.Modified;

            // Add new workout exercises
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
            var workout = await GetWorkoutById(id, adminId);
            if (workout != null)
            {
                _context.Workouts.Remove(workout);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> WorkoutExists(int id, string adminId)
        {
            return await _context.Workouts
                .AnyAsync(w => w.Id == id && w.AdminId == adminId);
        }
    }
}
