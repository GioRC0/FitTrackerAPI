namespace FitTrackerAPI.DTOs.Training;

public class PerformanceMetricsDto
{
    public double TechniquePercentage { get; set; }
    public double ConsistencyScore { get; set; }
    public double AverageConfidence { get; set; }
    public double? ControlScore { get; set; }
    public double? StabilityScore { get; set; }
    public double? AlignmentScore { get; set; }
    public double? BalanceScore { get; set; }
    public double? DepthScore { get; set; }
    public double? HipScore { get; set; }
    public double? CoreScore { get; set; }
    public double? ArmPositionScore { get; set; }
    public double? ResistanceScore { get; set; }
    public double? RepsPerMinute { get; set; }
}

