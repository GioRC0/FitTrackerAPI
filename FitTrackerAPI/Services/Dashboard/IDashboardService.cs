using FitTrackerAPI.DTOs.Dashboard;

namespace FitTrackerAPI.Services.Dashboard;

public interface IDashboardService
{
    Task<HomeDashboardDto> GetHomeDashboardAsync(string userId);
    Task<HomeStatsDto> GetHomeStatsAsync(string userId);
    Task<int> CalculateBestStreakAsync(string userId);
    Task<int> CalculateCurrentStreakAsync(string userId);
}

