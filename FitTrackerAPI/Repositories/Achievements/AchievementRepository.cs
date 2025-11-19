using FitTrackerAPI.Data;
using FitTrackerAPI.Models.Achievements;
using MongoDB.Driver;

namespace FitTrackerAPI.Repositories.Achievements;

public class AchievementRepository : IAchievementRepository
{
    private readonly IMongoCollection<Achievement> _achievementsCollection;
    private readonly IMongoCollection<UserAchievement> _userAchievementsCollection;

    public AchievementRepository(MongoDbContext context)
    {
        _achievementsCollection = context.Achievements;
        _userAchievementsCollection = context.UserAchievements;
    }

    public async Task<List<Achievement>> GetAllAchievementsAsync()
    {
        return await _achievementsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<List<UserAchievement>> GetUserAchievementsAsync(string userId)
    {
        return await _userAchievementsCollection
            .Find(ua => ua.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserAchievement?> GetUserAchievementAsync(string userId, string achievementId)
    {
        return await _userAchievementsCollection
            .Find(ua => ua.UserId == userId && ua.AchievementId == achievementId)
            .FirstOrDefaultAsync();
    }

    public async Task UnlockAchievementAsync(string userId, string achievementId)
    {
        // Verificar si ya está desbloqueado
        var existing = await GetUserAchievementAsync(userId, achievementId);
        if (existing != null) return;

        var userAchievement = new UserAchievement
        {
            UserId = userId,
            AchievementId = achievementId,
            EarnedAt = DateTime.UtcNow
        };

        await _userAchievementsCollection.InsertOneAsync(userAchievement);
    }

    public async Task SeedAchievementsAsync()
    {
        // Verificar si ya hay achievements
        var count = await _achievementsCollection.CountDocumentsAsync(_ => true);
        if (count > 0) return;

        var achievements = new List<Achievement>
        {
            new Achievement
            {
                Name = "Primera semana",
                Description = "Completaste tu primera semana de entrenamiento",
                Icon = "🎯",
                Category = "milestone",
                RequiredValue = 7
            },
            new Achievement
            {
                Name = "100 Push-ups",
                Description = "Alcanzaste 100 push-ups en total",
                Icon = "💪",
                Category = "volume",
                RequiredValue = 100
            },
            new Achievement
            {
                Name = "Constancia 7 días",
                Description = "Entrenaste 7 días seguidos",
                Icon = "🔥",
                Category = "streak",
                RequiredValue = 7
            },
            new Achievement
            {
                Name = "Mejorador",
                Description = "Supera tu mejor marca en 5 ejercicios diferentes",
                Icon = "📈",
                Category = "improvement",
                RequiredValue = 5
            },
            new Achievement
            {
                Name = "Madrugador",
                Description = "Entrena antes de las 7 AM en 5 ocasiones",
                Icon = "🌅",
                Category = "special",
                RequiredValue = 5
            },
            new Achievement
            {
                Name = "100 Plank segundos",
                Description = "Alcanzaste 100 segundos de plancha en total",
                Icon = "⏱️",
                Category = "volume",
                RequiredValue = 100
            },
            new Achievement
            {
                Name = "Primera sesión",
                Description = "Completaste tu primera sesión de entrenamiento",
                Icon = "🌟",
                Category = "milestone",
                RequiredValue = 1
            },
            new Achievement
            {
                Name = "Racha de 30 días",
                Description = "Entrenaste 30 días seguidos",
                Icon = "🔥🔥",
                Category = "streak",
                RequiredValue = 30
            },
            new Achievement
            {
                Name = "500 Repeticiones",
                Description = "Alcanzaste 500 repeticiones en total",
                Icon = "💯",
                Category = "volume",
                RequiredValue = 500
            },
            new Achievement
            {
                Name = "Constante",
                Description = "Entrena 30 días diferentes",
                Icon = "📅",
                Category = "milestone",
                RequiredValue = 30
            }
        };

        await _achievementsCollection.InsertManyAsync(achievements);
    }
}

