using hyperTROPHYbuddy.Data;
using hyperTROPHYbuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace hyperTROPHYbuddy.Services
{
    public class AuthorizationManagerService : IAuthorizationManagerService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorizationManagerService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> IsUserAdmin(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<bool> IsUserClient(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, "Client");
        }




    }
}

