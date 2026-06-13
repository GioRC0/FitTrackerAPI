using MongoDB.Bson.Serialization.Attributes;

namespace FitTrackerAPI.Models.Authentication;

public class PasswordResetToken
{
    [BsonElement("token")]
    public string Token { get; set; }

    [BsonElement("expiresAt")]
    public DateTime ExpiresAt { get; set; }

    [BsonElement("isUsed")]
    public bool IsUsed { get; set; }

    public bool IsActive => !IsUsed && ExpiresAt > DateTime.UtcNow;
}
