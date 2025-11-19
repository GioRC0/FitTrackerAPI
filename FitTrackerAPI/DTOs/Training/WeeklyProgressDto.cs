namespace FitTrackerAPI.DTOs.Training;

public class WeeklyProgressDto
{
    public string ExerciseId { get; set; } = string.Empty;
    public string ExerciseName { get; set; } = string.Empty;
    
    // Semana actual
    public WeekStats CurrentWeek { get; set; } = new();
    
    // Semana anterior
    public WeekStats PreviousWeek { get; set; } = new();
    
    // Comparación
    public ProgressComparison Comparison { get; set; } = new();
}

public class WeekStats
{
    public int TotalSessions { get; set; }
    public int TotalReps { get; set; }
    public int TotalSeconds { get; set; }
    public double AverageTechniquePercentage { get; set; }
    public double AverageConsistencyScore { get; set; }
    public double AverageConfidence { get; set; }
}

public class ProgressComparison
{
    public double SessionsChange { get; set; } // % de cambio
    public double RepsChange { get; set; }
    public double SecondsChange { get; set; }
    public double TechniqueChange { get; set; }
    public double ConsistencyChange { get; set; }
    public double ConfidenceChange { get; set; }
}

