using hyperTROPHYbuddy.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections;
using System.Security.Claims;

namespace hyperTROPHYbuddy.Services.Interfaces
{
    public interface IApplicationUserService
    {
        Task<IdentityResult> Register(ApplicationUser user, string password, List<string> roles);
        Task<SignInResult> Login(string username, string password, bool isPersistent, bool lockoutOnFailure);
        Task Logout();
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal principal);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<bool> IsUserInRoleAsync(ApplicationUser user, string role);
        Task<ApplicationUser> FindByUsernameAsync(string username);
        Task<IEnumerable> GetAllAsync();
    }
}
