﻿using FitTrackerAPI.Data;
using FitTrackerAPI.Models.Training;
using MongoDB.Driver;

namespace FitTrackerAPI.Repositories.Training;

public class TrainingSessionRepository : ITrainingSessionRepository
{
    private readonly IMongoCollection<TrainingSession> _trainingSessionCollection;

    public TrainingSessionRepository(MongoDbContext context)
    {
        _trainingSessionCollection = context.TrainingSessions;
        
        // Crear índices para mejorar el rendimiento
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var indexKeysDefinition = Builders<TrainingSession>.IndexKeys
            .Ascending(ts => ts.UserId)
            .Ascending(ts => ts.ExerciseId)
            .Descending(ts => ts.CreatedAt);

        var indexModel = new CreateIndexModel<TrainingSession>(indexKeysDefinition);
        
        _trainingSessionCollection.Indexes.CreateOneAsync(indexModel);
    }

    public async Task<TrainingSession> CreateAsync(TrainingSession session)
    {
        await _trainingSessionCollection.InsertOneAsync(session);
        return session;
    }

    public async Task<TrainingSession?> GetByIdAsync(string id)
    {
        return await _trainingSessionCollection
            .Find(ts => ts.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<TrainingSession>> GetByUserIdAsync(string userId, int skip, int limit)
    {
        return await _trainingSessionCollection
            .Find(ts => ts.UserId == userId)
            .SortByDescending(ts => ts.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<List<TrainingSession>> GetByUserAndExerciseAsync(string userId, string exerciseId, int skip, int limit)
    {
        return await _trainingSessionCollection
            .Find(ts => ts.UserId == userId && ts.ExerciseId == exerciseId)
            .SortByDescending(ts => ts.CreatedAt)
            .Skip(skip)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<List<TrainingSession>> GetWeeklySessionsAsync(string userId, string exerciseId, DateTime startDate, DateTime endDate)
    {
        return await _trainingSessionCollection
            .Find(ts => ts.UserId == userId 
                       && ts.ExerciseId == exerciseId 
                       && ts.CreatedAt >= startDate 
                       && ts.CreatedAt <= endDate)
            .SortByDescending(ts => ts.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<TrainingSession>> GetRecentSessionsAsync(string userId, string exerciseId, int limit)
    {
        return await _trainingSessionCollection
            .Find(ts => ts.UserId == userId && ts.ExerciseId == exerciseId)
            .SortByDescending(ts => ts.StartTime)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<List<TrainingSession>> GetSessionsByDateRangeAsync(string userId, string exerciseId, DateTime startDate, DateTime endDate)
    {
        if (string.IsNullOrEmpty(exerciseId))
        {
            // Si exerciseId está vacío, retornar todas las sesiones del usuario en el rango
            return await _trainingSessionCollection
                .Find(ts => ts.UserId == userId 
                           && ts.StartTime >= startDate 
                           && ts.StartTime <= endDate)
                .SortBy(ts => ts.StartTime)
                .ToListAsync();
        }
        
        return await _trainingSessionCollection
            .Find(ts => ts.UserId == userId 
                       && ts.ExerciseId == exerciseId 
                       && ts.StartTime >= startDate 
                       && ts.StartTime <= endDate)
            .SortBy(ts => ts.StartTime)
            .ToListAsync();
    }

    public async Task<List<TrainingSession>> GetAllSessionsByExerciseAsync(string userId, string exerciseId)
    {
        if (string.IsNullOrEmpty(exerciseId))
        {
            // Si exerciseId está vacío, retornar todas las sesiones del usuario
            return await _trainingSessionCollection
                .Find(ts => ts.UserId == userId)
                .SortBy(ts => ts.StartTime)
                .ToListAsync();
        }
        
        return await _trainingSessionCollection
            .Find(ts => ts.UserId == userId && ts.ExerciseId == exerciseId)
            .SortBy(ts => ts.StartTime)
            .ToListAsync();
    }

    public async Task<List<DateTime>> GetSessionDatesAsync(string userId, string exerciseId)
    {
        if (string.IsNullOrEmpty(exerciseId))
        {
            // Si exerciseId está vacío, retornar todas las fechas de sesiones del usuario
            var sessions = await _trainingSessionCollection
                .Find(ts => ts.UserId == userId)
                .Project(ts => ts.StartTime)
                .ToListAsync();
            
            return sessions;
        }
        
        var sessionsFiltered = await _trainingSessionCollection
            .Find(ts => ts.UserId == userId && ts.ExerciseId == exerciseId)
            .Project(ts => ts.StartTime)
            .ToListAsync();
        
        return sessionsFiltered;
    }
}

