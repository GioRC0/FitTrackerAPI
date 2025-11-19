using FitTrackerAPI.DTOs.Training;

namespace FitTrackerAPI.Services.Training;

public interface ITrainingSessionService
{
    Task<TrainingSessionResponseDto> CreateSessionAsync(TrainingSessionCreateDto dto, string userId);
    Task<TrainingSessionResponseDto?> GetSessionByIdAsync(string id, string userId);
    Task<List<TrainingSessionResponseDto>> GetUserSessionsAsync(string userId, int page, int pageSize, string? exerciseId = null);
    Task<WeeklyProgressDto> GetWeeklyProgressAsync(string userId, string exerciseId);
    Task<ExerciseStatsDto> GetExerciseStatsAsync(string userId, string exerciseId, int recentLimit);
    Task<ProgressDataDto> GetProgressDataAsync(string userId, string exerciseId, string range);
    Task<FormAnalysisDto> GetFormAnalysisAsync(string userId, string exerciseId, string range);
    Task<GoalsDto> GetGoalsAsync(string userId, string exerciseId);
}

