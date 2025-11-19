using MongoDB.Bson.Serialization.Attributes;

namespace FitTrackerAPI.Models.Authentication;

public class RefreshToken
{
    [BsonElement("token")]
    public required string Token { get; set; }

    [BsonElement("expires")]
    public DateTime Expires { get; set; }

    [BsonElement("created")]
    public DateTime Created { get; set; }

    [BsonIgnore] // No se guarda en DB, es un campo calculado
    public bool IsExpired => DateTime.UtcNow >= Expires;
    
    [BsonIgnore] // No se guarda en DB, es un campo calculado
    public bool IsActive => !IsExpired;
}