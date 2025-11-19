using FitTrackerAPI.DTOs.UserProfile;
using FitTrackerAPI.Models.Achievements;
using FitTrackerAPI.Models.Training;
using FitTrackerAPI.Repositories.Achievements;
using FitTrackerAPI.Repositories.Training;
using FitTrackerAPI.Repositories.Users;

namespace FitTrackerAPI.Services.UserProfile;

public class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly ITrainingSessionRepository _sessionRepository;
    private readonly IAchievementRepository _achievementRepository;

    public UserProfileService(
        IUserRepository userRepository,
        ITrainingSessionRepository sessionRepository,
        IAchievementRepository achievementRepository)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _achievementRepository = achievementRepository;
    }

    public async Task<UserProfileDto> GetUserProfileAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        
        if (user == null)
            throw new Exception("Usuario no encontrado");

        return new UserProfileDto
        {
            Id = user.Id,
            Name = user.Profile?.FirstName ?? "",
            LastName = user.Profile?.LastName ?? "",
            Email = user.Username,
            Weight = (double)(user.Profile?.Weight ?? 0),
            Height = (int)(user.Profile?.Height ?? 0),
            DateOfBirth = user.Profile?.DateOfBirth ?? DateTime.MinValue,
            CreatedAt = user.Profile?.CreatedAt ?? DateTime.UtcNow
        };
    }

    public async Task<UserStatsDto> GetUserStatsAsync(string userId)
    {
        var allSessions = await _sessionRepository.GetAllSessionsByExerciseAsync(userId, "");
        
        if (!allSessions.Any())
        {
            return new UserStatsDto
            {
                ActiveDays = 0,
                MasteredExercises = 0,
                CompletedGoals = 0,
                TotalWorkouts = 0,
                TotalReps = 0,
                TotalSeconds = 0,
                CurrentStreak = 0,
                BestStreak = 0
            };
        }

        // Active Days: Días únicos con al menos 1 sesión
        var activeDays = allSessions
            .Select(s => s.StartTime.Date)
            .Distinct()
            .Count();

        // Mastered Exercises: Ejercicios con al menos 10 sesiones
        var masteredExercises = allSessions
            .GroupBy(s => s.ExerciseId)
            .Count(g => g.Count() >= 10);

        // Total Workouts
        var totalWorkouts = allSessions.Count;

        // Total Reps (solo push-ups y squats)
        var totalReps = allSessions
            .Where(s => s.ExerciseType.ToLower() != "plank")
            .Sum(s => s.TotalReps);

        // Total Seconds (solo plank)
        var totalSeconds = allSessions
            .Where(s => s.ExerciseType.ToLower() == "plank")
            .Sum(s => s.TotalSeconds);

        // Current Streak y Best Streak
        var sessionDates = await _sessionRepository.GetSessionDatesAsync(userId, "");
        var currentStreak = CalculateCurrentStreak(sessionDates);
        var bestStreak = CalculateBestStreak(sessionDates);

        // Completed Goals: Contar achievements desbloqueados
        var userAchievements = await _achievementRepository.GetUserAchievementsAsync(userId);
        var completedGoals = userAchievements.Count;

        return new UserStatsDto
        {
            ActiveDays = activeDays,
            MasteredExercises = masteredExercises,
            CompletedGoals = completedGoals,
            TotalWorkouts = totalWorkouts,
            TotalReps = totalReps,
            TotalSeconds = totalSeconds,
            CurrentStreak = currentStreak,
            BestStreak = bestStreak
        };
    }

    public async Task<List<AchievementDto>> GetUserAchievementsAsync(string userId)
    {
        var allAchievements = await _achievementRepository.GetAllAchievementsAsync();
        var userAchievements = await _achievementRepository.GetUserAchievementsAsync(userId);

        var result = allAchievements.Select(a =>
        {
            var earned = userAchievements.FirstOrDefault(ua => ua.AchievementId == a.Id);
            return new AchievementDto
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                Icon = a.Icon,
                Earned = earned != null,
                EarnedAt = earned?.EarnedAt,
                Category = a.Category
            };
        }).ToList();

        // Ordenar: primero ganados (más recientes primero), luego no ganados
        return result
            .OrderByDescending(a => a.Earned)
            .ThenByDescending(a => a.EarnedAt)
            .ToList();
    }

    public async Task<UserProfileDto> UpdateUserProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        
        if (user == null)
            throw new Exception("Usuario no encontrado");

        if (user.Profile != null)
        {
            user.Profile.FirstName = request.Name;
            user.Profile.LastName = request.LastName;
            user.Profile.Weight = (decimal)request.Weight;
            user.Profile.Height = (decimal)request.Height;
        }

        await _userRepository.UpdateUserAsync(user);

        return await GetUserProfileAsync(userId);
    }

    public async Task CheckAndUnlockAchievementsAsync(string userId)
    {
        var allAchievements = await _achievementRepository.GetAllAchievementsAsync();
        var allSessions = await _sessionRepository.GetAllSessionsByExerciseAsync(userId, "");
        var sessionDates = await _sessionRepository.GetSessionDatesAsync(userId, "");

        foreach (var achievement in allAchievements)
        {
            // Verificar si ya está desbloqueado
            var existing = await _achievementRepository.GetUserAchievementAsync(userId, achievement.Id);
            if (existing != null) continue;

            bool shouldUnlock = false;

            switch (achievement.Category.ToLower())
            {
                case "milestone":
                    shouldUnlock = CheckMilestoneAchievement(achievement, allSessions, sessionDates);
                    break;

                case "volume":
                    shouldUnlock = CheckVolumeAchievement(achievement, allSessions);
                    break;

                case "streak":
                    shouldUnlock = CheckStreakAchievement(achievement, sessionDates);
                    break;

                case "improvement":
                    shouldUnlock = CheckImprovementAchievement(achievement, allSessions);
                    break;

                case "special":
                    shouldUnlock = CheckSpecialAchievement(achievement, allSessions);
                    break;
            }

            if (shouldUnlock)
            {
                await _achievementRepository.UnlockAchievementAsync(userId, achievement.Id);
            }
        }
    }

    // ========== HELPER METHODS ==========

    private int CalculateCurrentStreak(List<DateTime> sessionDates)
    {
        if (!sessionDates.Any()) return 0;

        var today = DateTime.UtcNow.Date;
        var dates = sessionDates.Select(d => d.Date).Distinct().OrderByDescending(d => d).ToList();

        if (dates[0] < today.AddDays(-1)) return 0;

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

    private int CalculateBestStreak(List<DateTime> sessionDates)
    {
        if (!sessionDates.Any()) return 0;

        var dates = sessionDates.Select(d => d.Date).Distinct().OrderBy(d => d).ToList();
        int maxStreak = 1;
        int currentStreak = 1;

        for (int i = 1; i < dates.Count; i++)
        {
            if ((dates[i] - dates[i - 1]).Days == 1)
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

    private bool CheckMilestoneAchievement(Achievement achievement, List<TrainingSession> sessions, List<DateTime> sessionDates)
    {
        if (achievement.Name.Contains("Primera sesión"))
        {
            return sessions.Count >= 1;
        }
        else if (achievement.Name.Contains("Primera semana"))
        {
            var activeDays = sessionDates.Select(d => d.Date).Distinct().Count();
            return activeDays >= achievement.RequiredValue;
        }
        else if (achievement.Name.Contains("Constante"))
        {
            var activeDays = sessionDates.Select(d => d.Date).Distinct().Count();
            return activeDays >= achievement.RequiredValue;
        }
        return false;
    }

    private bool CheckVolumeAchievement(Achievement achievement, List<TrainingSession> sessions)
    {
        if (achievement.Name.Contains("Push-ups"))
        {
            var totalPushups = sessions
                .Where(s => s.ExerciseType.ToLower() == "pushup")
                .Sum(s => s.TotalReps);
            return totalPushups >= achievement.RequiredValue;
        }
        else if (achievement.Name.Contains("Plank"))
        {
            var totalSeconds = sessions
                .Where(s => s.ExerciseType.ToLower() == "plank")
                .Sum(s => s.TotalSeconds);
            return totalSeconds >= achievement.RequiredValue;
        }
        else if (achievement.Name.Contains("Repeticiones"))
        {
            var totalReps = sessions
                .Where(s => s.ExerciseType.ToLower() != "plank")
                .Sum(s => s.TotalReps);
            return totalReps >= achievement.RequiredValue;
        }
        return false;
    }

    private bool CheckStreakAchievement(Achievement achievement, List<DateTime> sessionDates)
    {
        var bestStreak = CalculateBestStreak(sessionDates);
        return bestStreak >= achievement.RequiredValue;
    }

    private bool CheckImprovementAchievement(Achievement achievement, List<TrainingSession> sessions)
    {
        // "Mejorador": Supera mejor marca en 5 ejercicios diferentes
        var exercisesWithImprovement = sessions
            .GroupBy(s => s.ExerciseId)
            .Select(g => g.OrderBy(s => s.StartTime).ToList())
            .Count(exerciseSessions =>
            {
                if (exerciseSessions.Count < 2) return false;
                var first = exerciseSessions.First();
                var last = exerciseSessions.Last();
                
                if (first.ExerciseType.ToLower() == "plank")
                {
                    return last.TotalSeconds > first.TotalSeconds;
                }
                else
                {
                    return last.TotalReps > first.TotalReps;
                }
            });

        return exercisesWithImprovement >= achievement.RequiredValue;
    }

    private bool CheckSpecialAchievement(Achievement achievement, List<TrainingSession> sessions)
    {
        if (achievement.Name.Contains("Madrugador"))
        {
            var earlyMorningSessions = sessions
                .Count(s => s.StartTime.Hour < 7);
            return earlyMorningSessions >= achievement.RequiredValue;
        }
        return false;
    }
}

