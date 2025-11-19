using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitTrackerAPI.Models.Training;

public class TrainingSession
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = string.Empty;
    
    [BsonElement("exerciseId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ExerciseId { get; set; } = string.Empty;
    
    [BsonElement("exerciseType")]
    public string ExerciseType { get; set; } = string.Empty; // "pushup", "squat", "plank"
    
    [BsonElement("exerciseName")]
    public string ExerciseName { get; set; } = string.Empty;
    
    [BsonElement("startTime")]
    public DateTime StartTime { get; set; }
    
    [BsonElement("endTime")]
    public DateTime EndTime { get; set; }
    
    [BsonElement("durationSeconds")]
    public int DurationSeconds { get; set; }
    
    // Para PUSH-UPS y SQUATS
    [BsonElement("totalReps")]
    [BsonIgnoreIfDefault]
    public int TotalReps { get; set; }
    
    [BsonElement("repsData")]
    [BsonIgnoreIfNull]
    public List<RepData>? RepsData { get; set; }
    
    // Para PLANCHA
    [BsonElement("totalSeconds")]
    [BsonIgnoreIfDefault]
    public int TotalSeconds { get; set; }
    
    [BsonElement("secondsData")]
    [BsonIgnoreIfNull]
    public List<SecondData>? SecondsData { get; set; }
    
    // Métricas calculadas
    [BsonElement("metrics")]
    public PerformanceMetrics Metrics { get; set; } = new();
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

