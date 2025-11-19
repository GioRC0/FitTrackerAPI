namespace FitTrackerAPI.DTOs.Training;

public class ProgressDataDto
{
    public string TimeRange { get; set; } = string.Empty;
    public string ExerciseId { get; set; } = string.Empty;
    public string ExerciseName { get; set; } = string.Empty;
    public string ExerciseType { get; set; } = string.Empty;
    public List<ProgressDataPointDto> DataPoints { get; set; } = new();
    public ProgressSummaryDto Summary { get; set; } = new();
}

