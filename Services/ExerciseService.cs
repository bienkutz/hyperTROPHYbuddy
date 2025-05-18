using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace hyperTROPHYbuddy.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;

        public ExerciseService(
            ApplicationDbContext context,
            IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        public async Task<bool> IsUserAdmin(string userId) =>
            await _authorizationService.IsUserAdmin(userId);

        public async Task<IEnumerable<Exercise>> GetExercisesByAdmin(string adminId)
        {
            if (!await IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can view exercises");

            return await _context.Exercises
                .Where(e => e.AdminId == adminId)
                .Include(e => e.Admin)
                .ToListAsync();
        }

        public async Task<Exercise> GetExerciseById(int id, string adminId)
        {
            if (!await IsUserAdmin(adminId))
                throw new UnauthorizedAccessException("Only admins can view exercises");

            return await _context.Exercises
                .Include(e => e.Admin)
                .FirstOrDefaultAsync(e => e.Id == id && e.AdminId == adminId);
        }

        public async Task CreateExercise(Exercise exercise)
        {
            if (!await IsUserAdmin(exercise.AdminId))
                throw new UnauthorizedAccessException("Only admins can create exercises");

            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateExercise(Exercise exercise)
        {
            if (!await _authorizationService.CanModifyExercise(exercise.AdminId, exercise.Id))
                throw new UnauthorizedAccessException("Unauthorized to modify this exercise");

            _context.Entry(exercise).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExercise(int id, string adminId)
        {
            if (!await _authorizationService.CanModifyExercise(adminId, id))
                throw new UnauthorizedAccessException("Unauthorized to delete this exercise");

            var exercise = await GetExerciseById(id, adminId);
            if (exercise != null)
            {
                _context.Exercises.Remove(exercise);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExerciseExists(int id, string adminId)
        {
            if (!await IsUserAdmin(adminId))
                return false;

            return await _context.Exercises
                .AnyAsync(e => e.Id == id && e.AdminId == adminId);
        }
    }
}
