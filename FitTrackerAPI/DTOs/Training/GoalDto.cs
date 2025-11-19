namespace FitTrackerAPI.DTOs.Training;

public class GoalDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Current { get; set; }
    public int Target { get; set; }
    public double Progress { get; set; }
    public bool Achieved { get; set; }
}

