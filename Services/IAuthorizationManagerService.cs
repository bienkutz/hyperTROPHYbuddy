namespace hyperTROPHYbuddy.Services
{
    public interface IAuthorizationManagerService
    {
        Task<bool> IsUserAdmin(string userId);
        Task<bool> IsUserClient(string userId);
    }
}
