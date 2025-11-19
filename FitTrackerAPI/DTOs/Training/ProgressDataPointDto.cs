namespace FitTrackerAPI.DTOs.Training;

public class ProgressDataPointDto
{
    public string Label { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Reps { get; set; }
    public int Seconds { get; set; }
    public double AverageForm { get; set; }
}

