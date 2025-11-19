using MongoDB.Bson.Serialization.Attributes;

namespace FitTrackerAPI.Models.Training;

public class PerformanceMetrics
{
    [BsonElement("techniquePercentage")]
    public double TechniquePercentage { get; set; }
    
    [BsonElement("consistencyScore")]
    public double ConsistencyScore { get; set; }
    
    [BsonElement("averageConfidence")]
    public double AverageConfidence { get; set; }
    
    // Para push-ups y squats
    [BsonElement("controlScore")]
    [BsonIgnoreIfNull]
    public double? ControlScore { get; set; }
    
    [BsonElement("stabilityScore")]
    [BsonIgnoreIfNull]
    public double? StabilityScore { get; set; }
    
    // Para squats
    [BsonElement("alignmentScore")]
    [BsonIgnoreIfNull]
    public double? AlignmentScore { get; set; }
    
    [BsonElement("balanceScore")]
    [BsonIgnoreIfNull]
    public double? BalanceScore { get; set; }
    
    [BsonElement("depthScore")]
    [BsonIgnoreIfNull]
    public double? DepthScore { get; set; }
    
    // Para plank
    [BsonElement("hipScore")]
    [BsonIgnoreIfNull]
    public double? HipScore { get; set; }
    
    [BsonElement("coreScore")]
    [BsonIgnoreIfNull]
    public double? CoreScore { get; set; }
    
    [BsonElement("armPositionScore")]
    [BsonIgnoreIfNull]
    public double? ArmPositionScore { get; set; }
    
    [BsonElement("resistanceScore")]
    [BsonIgnoreIfNull]
    public double? ResistanceScore { get; set; }
    
    // Velocidad (para ejercicios con repeticiones)
    [BsonElement("repsPerMinute")]
    [BsonIgnoreIfNull]
    public double? RepsPerMinute { get; set; }
}

