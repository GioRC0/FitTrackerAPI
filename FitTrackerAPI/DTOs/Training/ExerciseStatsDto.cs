namespace FitTrackerAPI.DTOs.Training;

public class ExerciseStatsDto
{
    public WeeklySummaryDto WeeklySummary { get; set; } = new();
    public List<RecentSessionDto> RecentSessions { get; set; } = new();
}

