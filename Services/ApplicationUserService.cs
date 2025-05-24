using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using hyperTROPHYbuddy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Security.Claims;

namespace hyperTROPHYbuddy.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public ApplicationUserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IdentityResult> Register(ApplicationUser user, string password, List<string> roles)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return result;

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                    return roleResult;
            }
            return result;
        }

        public async Task<SignInResult> Login(string username, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return await _signInManager.PasswordSignInAsync(username, password, isPersistent, lockoutOnFailure);
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<bool> IsUserInRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<ApplicationUser> FindByUsernameAsync(string username)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<IEnumerable> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }
    }
}