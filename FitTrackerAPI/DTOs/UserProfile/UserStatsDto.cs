namespace FitTrackerAPI.DTOs.UserProfile;

public class UserStatsDto
{
    public int ActiveDays { get; set; }
    public int MasteredExercises { get; set; }
    public int CompletedGoals { get; set; }
    public int TotalWorkouts { get; set; }
    public int TotalReps { get; set; }
    public int TotalSeconds { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
}

