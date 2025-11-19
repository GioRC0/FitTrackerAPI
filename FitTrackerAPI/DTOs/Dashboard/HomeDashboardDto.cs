namespace FitTrackerAPI.DTOs.Dashboard;

public class HomeDashboardDto
{
    public UserSummaryDto User { get; set; } = new();
    public HomeStatsDto Stats { get; set; } = new();
    public RecentExerciseDto? LastExercise { get; set; }
    public List<RecentExerciseDto> RecentActivity { get; set; } = new();
}

