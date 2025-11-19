using FitTrackerAPI.DTOs.UserProfile;

namespace FitTrackerAPI.Services.UserProfile;

public interface IUserProfileService
{
    Task<UserProfileDto> GetUserProfileAsync(string userId);
    Task<UserStatsDto> GetUserStatsAsync(string userId);
    Task<List<AchievementDto>> GetUserAchievementsAsync(string userId);
    Task<UserProfileDto> UpdateUserProfileAsync(string userId, UpdateProfileRequest request);
    Task CheckAndUnlockAchievementsAsync(string userId);
}

