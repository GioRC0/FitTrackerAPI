using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitTrackerAPI.Models.Exercises;

public class Exercise
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("shortDescription")]
    public string ShortDescription { get; set; }

    [BsonElement("fullDescription")]
    public string FullDescription { get; set; }

    [BsonElement("difficulty")]
    public string Difficulty { get; set; }

    [BsonElement("muscleGroup")]
    public string MuscleGroup { get; set; }

    [BsonElement("minTime")]
    public int MinTime { get; set; }

    [BsonElement("maxTime")]
    public int MaxTime { get; set; }

    [BsonElement("steps")]
    public List<string> Steps { get; set; }

    [BsonElement("tips")]
    public List<string> Tips { get; set; }

    [BsonElement("imageUrl")]
    public string ImageUrl { get; set; }

    [BsonElement("shortImageUrl")]
    public string ShortImageUrl { get; set; }
}