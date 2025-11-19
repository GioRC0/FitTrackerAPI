using FitTrackerAPI.Models.Achievements;

namespace FitTrackerAPI.Repositories.Achievements;

public interface IAchievementRepository
{
    Task<List<Achievement>> GetAllAchievementsAsync();
    Task<List<UserAchievement>> GetUserAchievementsAsync(string userId);
    Task<UserAchievement?> GetUserAchievementAsync(string userId, string achievementId);
    Task UnlockAchievementAsync(string userId, string achievementId);
    Task SeedAchievementsAsync();
}

