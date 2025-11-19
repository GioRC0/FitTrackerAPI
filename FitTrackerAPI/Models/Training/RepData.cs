using MongoDB.Bson.Serialization.Attributes;

namespace FitTrackerAPI.Models.Training;

public class RepData
{
    [BsonElement("repNumber")]
    public int RepNumber { get; set; }
    
    [BsonElement("classification")]
    public string Classification { get; set; } = string.Empty;
    
    [BsonElement("confidence")]
    public double Confidence { get; set; }
    
    [BsonElement("probabilities")]
    public Dictionary<string, double> Probabilities { get; set; } = new();
    
    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }
}

