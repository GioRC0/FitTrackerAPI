namespace FitTrackerAPI.DTOs.Dashboard;

public class RecentExerciseDto
{
    public string Id { get; set; } = string.Empty;
    public string ExerciseId { get; set; } = string.Empty;
    public string ExerciseName { get; set; } = string.Empty;
    public string ExerciseType { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Reps { get; set; }
    public int Seconds { get; set; }
    public string Improvement { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

