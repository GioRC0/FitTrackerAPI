using FitTrackerAPI.Models.UserInfo;

namespace FitTrackerAPI.Repositories.Users;

public interface IUserRepository
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task AddUserAsync(User user);
    Task<User?> GetUserByIdAsync(string id);
    Task MarkUserAsVerifiedAsync(string userId);
    Task UpdateUserAsync(User user);
    Task<User?> GetUserByRefreshTokenAsync(string token); // <-- Añadir esta línea

}