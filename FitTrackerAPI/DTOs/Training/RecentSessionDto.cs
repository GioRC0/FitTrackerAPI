namespace FitTrackerAPI.DTOs.Training;

public class RecentSessionDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Reps { get; set; }
    public int Seconds { get; set; }
    public string Duration { get; set; } = string.Empty;
    public string QualityLabel { get; set; } = string.Empty;
    public double TechniquePercentage { get; set; }
}

