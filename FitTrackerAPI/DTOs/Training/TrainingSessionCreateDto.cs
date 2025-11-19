using System.ComponentModel.DataAnnotations;

namespace FitTrackerAPI.DTOs.Training;

public class TrainingSessionCreateDto
{
    [Required]
    public string ExerciseId { get; set; } = string.Empty;
    
    [Required]
    public string ExerciseType { get; set; } = string.Empty; // "pushup", "squat", "plank"
    
    [Required]
    public string ExerciseName { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public DateTime EndTime { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int DurationSeconds { get; set; }
    
    // Para PUSH-UPS y SQUATS
    public int? TotalReps { get; set; }
    public List<RepDataDto>? RepsData { get; set; }
    
    // Para PLANCHA
    public int? TotalSeconds { get; set; }
    public List<SecondDataDto>? SecondsData { get; set; }
    
    // Métricas calculadas
    [Required]
    public PerformanceMetricsDto Metrics { get; set; } = new();
}

