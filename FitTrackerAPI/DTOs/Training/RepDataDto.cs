namespace FitTrackerAPI.DTOs.Training;

public class RepDataDto
{
    public int RepNumber { get; set; }
    public string Classification { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public Dictionary<string, double> Probabilities { get; set; } = new();
    public DateTime Timestamp { get; set; }
}

