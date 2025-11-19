namespace FitTrackerAPI.DTOs.Training;

public class TrainingSessionResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ExerciseId { get; set; } = string.Empty;
    public string ExerciseType { get; set; } = string.Empty;
    public string ExerciseName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationSeconds { get; set; }
    public int TotalReps { get; set; }
    public List<RepDataDto>? RepsData { get; set; }
    public int TotalSeconds { get; set; }
    public List<SecondDataDto>? SecondsData { get; set; }
    public PerformanceMetricsDto Metrics { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

