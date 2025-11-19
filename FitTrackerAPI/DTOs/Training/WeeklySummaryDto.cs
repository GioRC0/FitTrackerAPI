namespace FitTrackerAPI.DTOs.Training;

public class WeeklySummaryDto
{
    public int TotalSessions { get; set; }
    public int TotalReps { get; set; }
    public int TotalSeconds { get; set; }
    public double AverageReps { get; set; }
    public double AverageSeconds { get; set; }
    public int BestSessionReps { get; set; }
    public int BestSessionSeconds { get; set; }
    public double ImprovementPercentage { get; set; }
    public string ExerciseType { get; set; } = string.Empty;
}

