namespace FitTrackerAPI.DTOs.Dashboard;

public class HomeStatsDto
{
    public int WeeklyWorkouts { get; set; }
    public int WeeklyTotalReps { get; set; }
    public int WeeklyTotalSeconds { get; set; }
    public int BestStreak { get; set; }
    public int CurrentStreak { get; set; }
}

