namespace FitTrackerAPI.DTOs.Training;

public class ProgressSummaryDto
{
    public int TotalReps { get; set; }
    public int TotalSeconds { get; set; }
    public int TotalSessions { get; set; }
    public int DaysWithActivity { get; set; }
    public double AveragePerDay { get; set; }
    public double AverageFormScore { get; set; }
    public BestDayDto BestDay { get; set; } = new();
    public double Improvement { get; set; }
    public string Consistency { get; set; } = string.Empty;
}

