namespace FitTrackerAPI.DTOs.Training;

public class GoalsDto
{
    public List<GoalDto> Goals { get; set; } = new();
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
}

