using FitTrackerAPI.DTOs.Dashboard;
using FitTrackerAPI.Models.Training;
using FitTrackerAPI.Repositories.Exercises;
using FitTrackerAPI.Repositories.Training;
using FitTrackerAPI.Repositories.Users;

namespace FitTrackerAPI.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly ITrainingSessionRepository _trainingSessionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IExerciseRepository _exerciseRepository;

    public DashboardService(
        ITrainingSessionRepository trainingSessionRepository,
        IUserRepository userRepository,
        IExerciseRepository exerciseRepository)
    {
        _trainingSessionRepository = trainingSessionRepository;
        _userRepository = userRepository;
        _exerciseRepository = exerciseRepository;
    }

    public async Task<HomeDashboardDto> GetHomeDashboardAsync(string userId)
    {
        // Obtener usuario
        var user = await _userRepository.GetUserByIdAsync(userId);
        var userName = user?.Profile?.FirstName ?? "Usuario";

        // Calcular estadísticas
        var stats = await GetHomeStatsAsync(userId);

        // Obtener todas las sesiones del usuario para calcular recientes
        var allSessions = await _trainingSessionRepository.GetAllSessionsByExerciseAsync(userId, "");
        
        // Obtener última sesión
        var lastSession = allSessions.OrderByDescending(s => s.StartTime).FirstOrDefault();
        RecentExerciseDto? lastExercise = null;

        if (lastSession != null)
        {
            var previousSession = await GetPreviousSessionAsync(userId, lastSession.ExerciseId, lastSession.StartTime);
            var imageUrl = await GetExerciseImageUrlAsync(lastSession.ExerciseId);

            lastExercise = new RecentExerciseDto
            {
                Id = lastSession.Id,
                ExerciseId = lastSession.ExerciseId,
                ExerciseName = lastSession.ExerciseName,
                ExerciseType = lastSession.ExerciseType,
                Date = lastSession.StartTime,
                Reps = lastSession.TotalReps,
                Seconds = lastSession.TotalSeconds,
                Improvement = CalculateImprovement(lastSession, previousSession),
                Duration = FormatDuration(lastSession.DurationSeconds),
                ImageUrl = imageUrl
            };
        }

        // Obtener últimas 3 sesiones para actividad reciente
        var recentSessions = allSessions.OrderByDescending(s => s.StartTime).Take(3).ToList();
        var recentActivity = new List<RecentExerciseDto>();

        foreach (var session in recentSessions)
        {
            var previousSession = await GetPreviousSessionAsync(userId, session.ExerciseId, session.StartTime);

            recentActivity.Add(new RecentExerciseDto
            {
                Id = session.Id,
                ExerciseId = session.ExerciseId,
                ExerciseName = session.ExerciseName,
                ExerciseType = session.ExerciseType,
                Date = session.StartTime,
                Reps = session.TotalReps,
                Seconds = session.TotalSeconds,
                Improvement = CalculateImprovement(session, previousSession),
                Duration = FormatDuration(session.DurationSeconds)
                // NO incluir ImageUrl en recentActivity
            });
        }

        return new HomeDashboardDto
        {
            User = new UserSummaryDto { Name = userName },
            Stats = stats,
            LastExercise = lastExercise,
            RecentActivity = recentActivity
        };
    }

    public async Task<HomeStatsDto> GetHomeStatsAsync(string userId)
    {
        var weekStart = DateTime.UtcNow.Date.AddDays(-6); // Últimos 7 días incluido hoy
        var sessions = await _trainingSessionRepository.GetSessionsByDateRangeAsync(userId, "", weekStart, DateTime.UtcNow);

        var weeklyWorkouts = sessions.Count;

        // Solo contar reps de ejercicios que no son plank
        var weeklyTotalReps = sessions
            .Where(s => s.ExerciseType.ToLower() != "plank")
            .Sum(s => s.TotalReps);

        // Solo contar segundos de ejercicios tipo plank
        var weeklyTotalSeconds = sessions
            .Where(s => s.ExerciseType.ToLower() == "plank")
            .Sum(s => s.TotalSeconds);

        return new HomeStatsDto
        {
            WeeklyWorkouts = weeklyWorkouts,
            WeeklyTotalReps = weeklyTotalReps,
            WeeklyTotalSeconds = weeklyTotalSeconds,
            BestStreak = await CalculateBestStreakAsync(userId),
            CurrentStreak = await CalculateCurrentStreakAsync(userId)
        };
    }

    public async Task<int> CalculateBestStreakAsync(string userId)
    {
        var sessionDates = await _trainingSessionRepository.GetSessionDatesAsync(userId, "");
        var dates = sessionDates.Select(d => d.Date).Distinct().OrderBy(d => d).ToList();

        if (dates.Count == 0) return 0;

        int maxStreak = 1;
        int currentStreak = 1;

        for (int i = 1; i < dates.Count; i++)
        {
            if ((dates[i] - dates[i - 1]).TotalDays == 1)
            {
                currentStreak++;
                maxStreak = Math.Max(maxStreak, currentStreak);
            }
            else
            {
                currentStreak = 1;
            }
        }

        return maxStreak;
    }

    public async Task<int> CalculateCurrentStreakAsync(string userId)
    {
        var sessionDates = await _trainingSessionRepository.GetSessionDatesAsync(userId, "");
        var today = DateTime.UtcNow.Date;
        var dates = sessionDates.Select(d => d.Date).Distinct().OrderByDescending(d => d).ToList();

        if (dates.Count == 0 || dates[0] < today.AddDays(-1)) return 0;

        int streak = 0;
        var currentDate = today;

        foreach (var date in dates)
        {
            if (date == currentDate || date == currentDate.AddDays(-1))
            {
                streak++;
                currentDate = date.AddDays(-1);
            }
            else
            {
                break;
            }
        }

        return streak;
    }

    // ========== HELPER METHODS ==========

    private async Task<TrainingSession?> GetPreviousSessionAsync(string userId, string exerciseId, DateTime currentSessionDate)
    {
        var sessions = await _trainingSessionRepository.GetAllSessionsByExerciseAsync(userId, exerciseId);
        return sessions
            .Where(s => s.StartTime < currentSessionDate)
            .OrderByDescending(s => s.StartTime)
            .FirstOrDefault();
    }

    private async Task<string?> GetExerciseImageUrlAsync(string exerciseId)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(exerciseId);
        return exercise?.ShortImageUrl;
    }

    private string CalculateImprovement(TrainingSession current, TrainingSession? previous)
    {
        if (previous == null) return "Primera sesión";

        if (current.ExerciseType.ToLower() == "plank")
        {
            var diff = current.TotalSeconds - previous.TotalSeconds;
            return diff >= 0 ? $"+{diff} seg" : $"{diff} seg";
        }
        else
        {
            var diff = current.TotalReps - previous.TotalReps;
            return diff >= 0 ? $"+{diff} reps" : $"{diff} reps";
        }
    }

    private string FormatDuration(int durationSeconds)
    {
        if (durationSeconds < 60)
        {
            return $"{durationSeconds} seg";
        }
        else
        {
            var minutes = Math.Round(durationSeconds / 60.0);
            return $"{minutes} min";
        }
    }
}
