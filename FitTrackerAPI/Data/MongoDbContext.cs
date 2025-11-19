﻿﻿using FitTrackerAPI.Models.Achievements;
using FitTrackerAPI.Models.Training;
using FitTrackerAPI.Models.UserInfo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FitTrackerAPI.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<TrainingSession> TrainingSessions => _database.GetCollection<TrainingSession>("TrainingSessions");
    public IMongoCollection<Achievement> Achievements => _database.GetCollection<Achievement>("Achievements");
    public IMongoCollection<UserAchievement> UserAchievements => _database.GetCollection<UserAchievement>("UserAchievements");
}