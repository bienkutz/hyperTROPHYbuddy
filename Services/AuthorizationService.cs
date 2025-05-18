using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace hyperTROPHYbuddy.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AuthorizationService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> IsUserAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<bool> CanAccessClientPlan(string userId, int planId)
        {
            return await _context.ClientWorkoutPlans
                .AnyAsync(cwp => cwp.ClientId == userId && cwp.Id == planId);
        }

        public async Task<bool> CanModifyExercise(string userId, int exerciseId)
        {
            var isAdmin = await IsUserAdmin(userId);
            if (!isAdmin) return false;

            return await _context.Exercises
                .AnyAsync(e => e.Id == exerciseId && e.AdminId == userId);
        }
        public async Task<bool> CanModifyWorkout(string userId, int workoutId)
        {
            var isAdmin = await IsUserAdmin(userId);
            if (!isAdmin) return false;

            return await _context.Workouts
                .AnyAsync(w => w.Id == workoutId && w.AdminId == userId);
        }

        public async Task<bool> CanModifyPlan(string userId, int planId)
        {
            var isAdmin = await IsUserAdmin(userId);
            if (!isAdmin) return false;

            return await _context.WorkoutPlans
                .AnyAsync(p => p.Id == planId && p.AdminId == userId);
        }

        public async Task<bool> IsValidClient(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.IsInRoleAsync(user, "Client");
        }
    }
}

