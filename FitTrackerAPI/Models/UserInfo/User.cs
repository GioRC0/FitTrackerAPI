using System.Collections.Generic;
using FitTrackerAPI.Models.Authentication;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FitTrackerAPI.Models.UserInfo;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("username")]
    public string Username { get; set; } // Email

    [BsonElement("role")]
    public string Role { get; set; } = "User";

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; }

    [BsonElement("isVerified")]
    public bool IsVerified { get; set; } = false;

    // En NoSQL, es común anidar documentos relacionados
    [BsonElement("profile")]
    public UserProfile Profile { get; set; }

    [BsonElement("refreshTokens")]
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [BsonElement("validationCodes")]
    public ICollection<ValidationCode> ValidationCodes { get; set; } = new List<ValidationCode>();
}