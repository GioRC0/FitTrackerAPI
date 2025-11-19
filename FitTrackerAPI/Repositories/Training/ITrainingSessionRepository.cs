using FitTrackerAPI.Models.Training;

namespace FitTrackerAPI.Repositories.Training;

public interface ITrainingSessionRepository
{
    Task<TrainingSession> CreateAsync(TrainingSession session);
    Task<TrainingSession?> GetByIdAsync(string id);
    Task<List<TrainingSession>> GetByUserIdAsync(string userId, int skip, int limit);
    Task<List<TrainingSession>> GetByUserAndExerciseAsync(string userId, string exerciseId, int skip, int limit);
    Task<List<TrainingSession>> GetWeeklySessionsAsync(string userId, string exerciseId, DateTime startDate, DateTime endDate);
    Task<List<TrainingSession>> GetRecentSessionsAsync(string userId, string exerciseId, int limit);
    Task<List<TrainingSession>> GetSessionsByDateRangeAsync(string userId, string exerciseId, DateTime startDate, DateTime endDate);
    Task<List<TrainingSession>> GetAllSessionsByExerciseAsync(string userId, string exerciseId);
    Task<List<DateTime>> GetSessionDatesAsync(string userId, string exerciseId);
}

