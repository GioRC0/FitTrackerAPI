namespace FitTrackerAPI.DTOs.Training;

public class FormAnalysisDto
{
    public double AverageScore { get; set; }
    public List<AspectScoreDto> AspectScores { get; set; } = new();
    public List<TrendPointDto> Trend { get; set; } = new();
}

