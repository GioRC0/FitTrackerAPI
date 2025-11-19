using FitTrackerAPI.DTOs.Training;
using FitTrackerAPI.Models.Training;
using FitTrackerAPI.Repositories.Training;

namespace FitTrackerAPI.Services.Training;

public class TrainingSessionService : ITrainingSessionService
{
    private readonly ITrainingSessionRepository _trainingSessionRepository;

    public TrainingSessionService(ITrainingSessionRepository trainingSessionRepository)
    {
        _trainingSessionRepository = trainingSessionRepository;
    }

    public async Task<TrainingSessionResponseDto> CreateSessionAsync(TrainingSessionCreateDto dto, string userId)
    {
        var session = new TrainingSession
        {
            UserId = userId,
            ExerciseId = dto.ExerciseId,
            ExerciseType = dto.ExerciseType.ToLower(),
            ExerciseName = dto.ExerciseName,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            DurationSeconds = dto.DurationSeconds,
            TotalReps = dto.TotalReps ?? 0,
            RepsData = dto.RepsData?.Select(r => new RepData
            {
                RepNumber = r.RepNumber,
                Classification = r.Classification,
                Confidence = r.Confidence,
                Probabilities = r.Probabilities,
                Timestamp = r.Timestamp
            }).ToList(),
            TotalSeconds = dto.TotalSeconds ?? 0,
            SecondsData = dto.SecondsData?.Select(s => new SecondData
            {
                SecondNumber = s.SecondNumber,
                Classification = s.Classification,
                Confidence = s.Confidence,
                Probabilities = s.Probabilities,
                Timestamp = s.Timestamp
            }).ToList(),
            Metrics = new PerformanceMetrics
            {
                TechniquePercentage = dto.Metrics.TechniquePercentage,
                ConsistencyScore = dto.Metrics.ConsistencyScore,
                AverageConfidence = dto.Metrics.AverageConfidence,
                ControlScore = dto.Metrics.ControlScore,
                StabilityScore = dto.Metrics.StabilityScore,
                AlignmentScore = dto.Metrics.AlignmentScore,
                BalanceScore = dto.Metrics.BalanceScore,
                DepthScore = dto.Metrics.DepthScore,
                HipScore = dto.Metrics.HipScore,
                CoreScore = dto.Metrics.CoreScore,
                ArmPositionScore = dto.Metrics.ArmPositionScore,
                ResistanceScore = dto.Metrics.ResistanceScore,
                RepsPerMinute = dto.Metrics.RepsPerMinute
            },
            CreatedAt = DateTime.UtcNow
        };

        var createdSession = await _trainingSessionRepository.CreateAsync(session);
        return MapToResponseDto(createdSession);
    }

    public async Task<TrainingSessionResponseDto?> GetSessionByIdAsync(string id, string userId)
    {
        var session = await _trainingSessionRepository.GetByIdAsync(id);
        
        if (session == null || session.UserId != userId)
        {
            return null;
        }

        return MapToResponseDto(session);
    }

    public async Task<List<TrainingSessionResponseDto>> GetUserSessionsAsync(string userId, int page, int pageSize, string? exerciseId = null)
    {
        var skip = (page - 1) * pageSize;
        
        List<TrainingSession> sessions;
        
        if (!string.IsNullOrEmpty(exerciseId))
        {
            sessions = await _trainingSessionRepository.GetByUserAndExerciseAsync(userId, exerciseId, skip, pageSize);
        }
        else
        {
            sessions = await _trainingSessionRepository.GetByUserIdAsync(userId, skip, pageSize);
        }

        return sessions.Select(MapToResponseDto).ToList();
    }

    public async Task<WeeklyProgressDto> GetWeeklyProgressAsync(string userId, string exerciseId)
    {
        var now = DateTime.UtcNow;
        
        // Semana actual (últimos 7 días)
        var currentWeekStart = now.AddDays(-7);
        var currentWeekEnd = now;
        
        // Semana anterior (días 8-14 atrás)
        var previousWeekStart = now.AddDays(-14);
        var previousWeekEnd = now.AddDays(-7);

        var currentWeekSessions = await _trainingSessionRepository.GetWeeklySessionsAsync(
            userId, exerciseId, currentWeekStart, currentWeekEnd);
        
        var previousWeekSessions = await _trainingSessionRepository.GetWeeklySessionsAsync(
            userId, exerciseId, previousWeekStart, previousWeekEnd);

        var currentStats = CalculateWeekStats(currentWeekSessions);
        var previousStats = CalculateWeekStats(previousWeekSessions);
        
        var comparison = CalculateComparison(currentStats, previousStats);

        return new WeeklyProgressDto
        {
            ExerciseId = exerciseId,
            ExerciseName = currentWeekSessions.FirstOrDefault()?.ExerciseName ?? "",
            CurrentWeek = currentStats,
            PreviousWeek = previousStats,
            Comparison = comparison
        };
    }

    private WeekStats CalculateWeekStats(List<TrainingSession> sessions)
    {
        if (!sessions.Any())
        {
            return new WeekStats();
        }

        return new WeekStats
        {
            TotalSessions = sessions.Count,
            TotalReps = sessions.Sum(s => s.TotalReps),
            TotalSeconds = sessions.Sum(s => s.TotalSeconds),
            AverageTechniquePercentage = sessions.Average(s => s.Metrics.TechniquePercentage),
            AverageConsistencyScore = sessions.Average(s => s.Metrics.ConsistencyScore),
            AverageConfidence = sessions.Average(s => s.Metrics.AverageConfidence)
        };
    }

    private ProgressComparison CalculateComparison(WeekStats current, WeekStats previous)
    {
        return new ProgressComparison
        {
            SessionsChange = CalculatePercentageChange(previous.TotalSessions, current.TotalSessions),
            RepsChange = CalculatePercentageChange(previous.TotalReps, current.TotalReps),
            SecondsChange = CalculatePercentageChange(previous.TotalSeconds, current.TotalSeconds),
            TechniqueChange = CalculatePercentageChange(previous.AverageTechniquePercentage, current.AverageTechniquePercentage),
            ConsistencyChange = CalculatePercentageChange(previous.AverageConsistencyScore, current.AverageConsistencyScore),
            ConfidenceChange = CalculatePercentageChange(previous.AverageConfidence, current.AverageConfidence)
        };
    }

    private double CalculatePercentageChange(double oldValue, double newValue)
    {
        if (oldValue == 0) return newValue > 0 ? 100 : 0;
        return ((newValue - oldValue) / oldValue) * 100;
    }

    private TrainingSessionResponseDto MapToResponseDto(TrainingSession session)
    {
        return new TrainingSessionResponseDto
        {
            Id = session.Id,
            UserId = session.UserId,
            ExerciseId = session.ExerciseId,
            ExerciseType = session.ExerciseType,
            ExerciseName = session.ExerciseName,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            DurationSeconds = session.DurationSeconds,
            TotalReps = session.TotalReps,
            RepsData = session.RepsData?.Select(r => new RepDataDto
            {
                RepNumber = r.RepNumber,
                Classification = r.Classification,
                Confidence = r.Confidence,
                Probabilities = r.Probabilities,
                Timestamp = r.Timestamp
            }).ToList(),
            TotalSeconds = session.TotalSeconds,
            SecondsData = session.SecondsData?.Select(s => new SecondDataDto
            {
                SecondNumber = s.SecondNumber,
                Classification = s.Classification,
                Confidence = s.Confidence,
                Probabilities = s.Probabilities,
                Timestamp = s.Timestamp
            }).ToList(),
            Metrics = new PerformanceMetricsDto
            {
                TechniquePercentage = session.Metrics.TechniquePercentage,
                ConsistencyScore = session.Metrics.ConsistencyScore,
                AverageConfidence = session.Metrics.AverageConfidence,
                ControlScore = session.Metrics.ControlScore,
                StabilityScore = session.Metrics.StabilityScore,
                AlignmentScore = session.Metrics.AlignmentScore,
                BalanceScore = session.Metrics.BalanceScore,
                DepthScore = session.Metrics.DepthScore,
                HipScore = session.Metrics.HipScore,
                CoreScore = session.Metrics.CoreScore,
                ArmPositionScore = session.Metrics.ArmPositionScore,
                ResistanceScore = session.Metrics.ResistanceScore,
                RepsPerMinute = session.Metrics.RepsPerMinute
            },
            CreatedAt = session.CreatedAt
        };
    }

    public async Task<ExerciseStatsDto> GetExerciseStatsAsync(string userId, string exerciseId, int recentLimit)
    {
        var today = DateTime.UtcNow;
        var weekStart = today.AddDays(-7);
        var previousWeekStart = today.AddDays(-14);

        // Sesiones de esta semana
        var currentWeekSessions = await _trainingSessionRepository.GetWeeklySessionsAsync(
            userId, exerciseId, weekStart, today);

        // Sesiones de semana anterior
        var previousWeekSessions = await _trainingSessionRepository.GetWeeklySessionsAsync(
            userId, exerciseId, previousWeekStart, weekStart);

        // Sesiones recientes
        var recentSessions = await _trainingSessionRepository.GetRecentSessionsAsync(
            userId, exerciseId, recentLimit);

        // Construir resumen semanal
        var weeklySummary = BuildWeeklySummary(currentWeekSessions, previousWeekSessions);

        // Construir sesiones recientes
        var recentSessionsList = recentSessions.Select(s => new RecentSessionDto
        {
            Id = s.Id,
            Date = s.StartTime,
            Reps = s.TotalReps,
            Seconds = s.TotalSeconds,
            Duration = FormatDuration(s.DurationSeconds),
            QualityLabel = GetQualityLabel(s.Metrics.TechniquePercentage),
            TechniquePercentage = Math.Round(s.Metrics.TechniquePercentage, 1)
        }).ToList();

        return new ExerciseStatsDto
        {
            WeeklySummary = weeklySummary,
            RecentSessions = recentSessionsList
        };
    }

    private WeeklySummaryDto BuildWeeklySummary(List<TrainingSession> currentWeekSessions, List<TrainingSession> previousWeekSessions)
    {
        if (!currentWeekSessions.Any())
        {
            return new WeeklySummaryDto
            {
                TotalSessions = 0,
                TotalReps = 0,
                TotalSeconds = 0,
                AverageReps = 0,
                AverageSeconds = 0,
                BestSessionReps = 0,
                BestSessionSeconds = 0,
                ImprovementPercentage = 0,
                ExerciseType = "unknown"
            };
        }

        var exerciseType = currentWeekSessions.First().ExerciseType;
        var isPlank = exerciseType.ToLower() == "plank";

        var totalSessions = currentWeekSessions.Count;
        var totalReps = isPlank ? 0 : currentWeekSessions.Sum(s => s.TotalReps);
        var totalSeconds = isPlank ? currentWeekSessions.Sum(s => s.TotalSeconds) : 0;
        var averageReps = isPlank ? 0 : (double)totalReps / totalSessions;
        var averageSeconds = isPlank ? (double)totalSeconds / totalSessions : 0;
        var bestSessionReps = isPlank ? 0 : currentWeekSessions.Max(s => s.TotalReps);
        var bestSessionSeconds = isPlank ? currentWeekSessions.Max(s => s.TotalSeconds) : 0;

        // Calcular mejora
        double improvementPercentage = 0;
        if (previousWeekSessions.Any())
        {
            if (isPlank)
            {
                var prevAvgSeconds = (double)previousWeekSessions.Sum(s => s.TotalSeconds) / previousWeekSessions.Count;
                if (prevAvgSeconds > 0)
                {
                    improvementPercentage = ((averageSeconds - prevAvgSeconds) / prevAvgSeconds) * 100;
                }
            }
            else
            {
                var prevAvgReps = (double)previousWeekSessions.Sum(s => s.TotalReps) / previousWeekSessions.Count;
                if (prevAvgReps > 0)
                {
                    improvementPercentage = ((averageReps - prevAvgReps) / prevAvgReps) * 100;
                }
            }
        }

        return new WeeklySummaryDto
        {
            TotalSessions = totalSessions,
            TotalReps = totalReps,
            TotalSeconds = totalSeconds,
            AverageReps = Math.Round(averageReps, 1),
            AverageSeconds = Math.Round(averageSeconds, 1),
            BestSessionReps = bestSessionReps,
            BestSessionSeconds = bestSessionSeconds,
            ImprovementPercentage = Math.Round(improvementPercentage, 1),
            ExerciseType = exerciseType
        };
    }

    private string FormatDuration(int seconds)
    {
        if (seconds >= 60)
        {
            var minutes = seconds / 60;
            return $"{minutes} min";
        }
        return $"{seconds} seg";
    }

    private string GetQualityLabel(double techniquePercentage)
    {
        if (techniquePercentage >= 85) return "Excelente";
        if (techniquePercentage >= 70) return "Buena";
        if (techniquePercentage >= 50) return "Regular";
        return "Mala";
    }

    // ========== PROGRESS DASHBOARD METHODS ==========

    public async Task<ProgressDataDto> GetProgressDataAsync(string userId, string exerciseId, string range)
    {
        var (startDate, endDate) = GetDateRange(range);
        var sessions = await _trainingSessionRepository.GetSessionsByDateRangeAsync(userId, exerciseId, startDate, endDate);

        if (!sessions.Any())
        {
            return new ProgressDataDto
            {
                TimeRange = range,
                ExerciseId = exerciseId,
                ExerciseName = "",
                ExerciseType = "unknown",
                DataPoints = new List<ProgressDataPointDto>(),
                Summary = new ProgressSummaryDto()
            };
        }

        var exerciseType = sessions.First().ExerciseType;
        var exerciseName = sessions.First().ExerciseName;
        var isPlank = exerciseType.ToLower() == "plank";

        // Agrupar sesiones por día o semana
        var dataPoints = range.ToLower() == "week" 
            ? GroupByDays(sessions, startDate, endDate, isPlank)
            : GroupByWeeks(sessions, startDate, endDate, isPlank);

        // Calcular resumen
        var summary = CalculateProgressSummary(sessions, dataPoints, isPlank, range);

        return new ProgressDataDto
        {
            TimeRange = range,
            ExerciseId = exerciseId,
            ExerciseName = exerciseName,
            ExerciseType = exerciseType,
            DataPoints = dataPoints,
            Summary = summary
        };
    }

    public async Task<FormAnalysisDto> GetFormAnalysisAsync(string userId, string exerciseId, string range)
    {
        var (startDate, endDate) = GetDateRange(range);
        var sessions = await _trainingSessionRepository.GetSessionsByDateRangeAsync(userId, exerciseId, startDate, endDate);

        if (!sessions.Any())
        {
            return new FormAnalysisDto
            {
                AverageScore = 0,
                AspectScores = new List<AspectScoreDto>(),
                Trend = new List<TrendPointDto>()
            };
        }

        var exerciseType = sessions.First().ExerciseType.ToLower();
        var averageScore = sessions.Average(s => s.Metrics.TechniquePercentage);

        // Mapear aspectos según tipo de ejercicio
        var aspectScores = GetAspectScores(sessions, exerciseType);

        // Calcular tendencia diaria
        var trend = sessions
            .GroupBy(s => s.StartTime.Date)
            .Select(g => new TrendPointDto
            {
                Date = g.Key,
                Score = Math.Round(g.Average(s => s.Metrics.TechniquePercentage), 1)
            })
            .OrderBy(t => t.Date)
            .ToList();

        return new FormAnalysisDto
        {
            AverageScore = Math.Round(averageScore, 1),
            AspectScores = aspectScores,
            Trend = trend
        };
    }

    public async Task<GoalsDto> GetGoalsAsync(string userId, string exerciseId)
    {
        var allSessions = await _trainingSessionRepository.GetAllSessionsByExerciseAsync(userId, exerciseId);
        var sessionDates = await _trainingSessionRepository.GetSessionDatesAsync(userId, exerciseId);

        if (!allSessions.Any())
        {
            return new GoalsDto
            {
                Goals = new List<GoalDto>(),
                CurrentStreak = 0,
                LongestStreak = 0
            };
        }

        var isPlank = allSessions.First().ExerciseType.ToLower() == "plank";
        var currentStreak = CalculateCurrentStreak(sessionDates);
        var longestStreak = CalculateLongestStreak(sessionDates);

        // Total de reps/segundos
        var totalValue = isPlank 
            ? allSessions.Sum(s => s.TotalSeconds)
            : allSessions.Sum(s => s.TotalReps);

        // Técnica promedio de últimas 10 sesiones
        var recentSessions = allSessions.OrderByDescending(s => s.StartTime).Take(10);
        var averageTechnique = recentSessions.Any() 
            ? recentSessions.Average(s => s.Metrics.TechniquePercentage)
            : 0;

        var goals = new List<GoalDto>
        {
            new GoalDto
            {
                Id = "streak_10_days",
                Title = "Racha de 10 días",
                Description = "Entrena 10 días consecutivos",
                Current = currentStreak,
                Target = 10,
                Progress = Math.Min((currentStreak / 10.0) * 100, 100),
                Achieved = currentStreak >= 10
            },
            new GoalDto
            {
                Id = isPlank ? "total_seconds_200" : "total_reps_200",
                Title = isPlank ? "200 segundos totales" : "200 repeticiones totales",
                Description = isPlank ? "Alcanza 200 segundos en total" : "Alcanza 200 reps en total",
                Current = totalValue,
                Target = 200,
                Progress = Math.Min((totalValue / 200.0) * 100, 100),
                Achieved = totalValue >= 200
            },
            new GoalDto
            {
                Id = "perfect_technique",
                Title = "Técnica perfecta",
                Description = "Alcanza 95 puntos de técnica promedio",
                Current = (int)Math.Round(averageTechnique),
                Target = 95,
                Progress = Math.Min((averageTechnique / 95.0) * 100, 100),
                Achieved = averageTechnique >= 95
            }
        };

        return new GoalsDto
        {
            Goals = goals,
            CurrentStreak = currentStreak,
            LongestStreak = longestStreak
        };
    }

    // ========== HELPER METHODS ==========

    private (DateTime startDate, DateTime endDate) GetDateRange(string range)
    {
        var endDate = DateTime.UtcNow;
        var startDate = range.ToLower() == "week"
            ? endDate.AddDays(-6).Date // Últimos 7 días (incluyendo hoy)
            : endDate.AddDays(-27).Date; // Últimos 28 días (4 semanas)

        return (startDate, endDate);
    }

    private List<ProgressDataPointDto> GroupByDays(List<TrainingSession> sessions, DateTime startDate, DateTime endDate, bool isPlank)
    {
        var dataPoints = new List<ProgressDataPointDto>();
        var dayLabels = new[] { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var daySessions = sessions.Where(s => s.StartTime.Date == date).ToList();
            
            dataPoints.Add(new ProgressDataPointDto
            {
                Label = dayLabels[(int)date.DayOfWeek],
                Date = date,
                Reps = isPlank ? 0 : daySessions.Sum(s => s.TotalReps),
                Seconds = isPlank ? daySessions.Sum(s => s.TotalSeconds) : 0,
                AverageForm = daySessions.Any() 
                    ? Math.Round(daySessions.Average(s => s.Metrics.TechniquePercentage), 1)
                    : 0
            });
        }

        return dataPoints;
    }

    private List<ProgressDataPointDto> GroupByWeeks(List<TrainingSession> sessions, DateTime startDate, DateTime endDate, bool isPlank)
    {
        var dataPoints = new List<ProgressDataPointDto>();
        var weekNumber = 1;

        for (var weekStart = startDate; weekStart <= endDate; weekStart = weekStart.AddDays(7))
        {
            var weekEnd = weekStart.AddDays(6);
            if (weekEnd > endDate) weekEnd = endDate;

            var weekSessions = sessions.Where(s => s.StartTime.Date >= weekStart && s.StartTime.Date <= weekEnd).ToList();

            dataPoints.Add(new ProgressDataPointDto
            {
                Label = $"Sem {weekNumber}",
                Date = weekStart,
                Reps = isPlank ? 0 : weekSessions.Sum(s => s.TotalReps),
                Seconds = isPlank ? weekSessions.Sum(s => s.TotalSeconds) : 0,
                AverageForm = weekSessions.Any()
                    ? Math.Round(weekSessions.Average(s => s.Metrics.TechniquePercentage), 1)
                    : 0
            });

            weekNumber++;
        }

        return dataPoints;
    }

    private ProgressSummaryDto CalculateProgressSummary(List<TrainingSession> sessions, List<ProgressDataPointDto> dataPoints, bool isPlank, string range)
    {
        var totalReps = isPlank ? 0 : sessions.Sum(s => s.TotalReps);
        var totalSeconds = isPlank ? sessions.Sum(s => s.TotalSeconds) : 0;
        var totalSessions = sessions.Count;
        var daysWithActivity = sessions.GroupBy(s => s.StartTime.Date).Count();
        var averageFormScore = sessions.Any() ? Math.Round(sessions.Average(s => s.Metrics.TechniquePercentage), 1) : 0;

        // Mejor día
        var bestPoint = dataPoints.OrderByDescending(d => isPlank ? d.Seconds : d.Reps).FirstOrDefault();
        var bestDay = new BestDayDto
        {
            Label = bestPoint?.Label ?? "",
            Value = isPlank ? (bestPoint?.Seconds ?? 0) : (bestPoint?.Reps ?? 0)
        };

        // Promedio por día
        var totalDays = range.ToLower() == "week" ? 7 : 28;
        var averagePerDay = isPlank 
            ? Math.Round((double)totalSeconds / totalDays, 1)
            : Math.Round((double)totalReps / totalDays, 1);

        // Calcular mejora (primera semana vs última semana)
        var improvement = CalculateImprovement(dataPoints, isPlank, range);

        // Determinar consistencia
        var consistency = GetConsistencyLabel(daysWithActivity, range);

        return new ProgressSummaryDto
        {
            TotalReps = totalReps,
            TotalSeconds = totalSeconds,
            TotalSessions = totalSessions,
            DaysWithActivity = daysWithActivity,
            AveragePerDay = averagePerDay,
            AverageFormScore = averageFormScore,
            BestDay = bestDay,
            Improvement = improvement,
            Consistency = consistency
        };
    }

    private double CalculateImprovement(List<ProgressDataPointDto> dataPoints, bool isPlank, string range)
    {
        if (!dataPoints.Any()) return 0;

        if (range.ToLower() == "week")
        {
            // Comparar primeros 3 días vs últimos 3 días
            var firstThreeDays = dataPoints.Take(3).ToList();
            var lastThreeDays = dataPoints.Skip(Math.Max(0, dataPoints.Count - 3)).ToList();

            // Filtrar días con datos
            var firstDaysWithData = isPlank 
                ? firstThreeDays.Where(d => d.Seconds > 0).ToList()
                : firstThreeDays.Where(d => d.Reps > 0).ToList();

            var lastDaysWithData = isPlank
                ? lastThreeDays.Where(d => d.Seconds > 0).ToList()
                : lastThreeDays.Where(d => d.Reps > 0).ToList();

            // Verificar que haya datos en ambos períodos
            if (!firstDaysWithData.Any() || !lastDaysWithData.Any()) return 0;

            var firstAvg = isPlank 
                ? firstDaysWithData.Average(d => (double)d.Seconds)
                : firstDaysWithData.Average(d => (double)d.Reps);

            var lastAvg = isPlank
                ? lastDaysWithData.Average(d => (double)d.Seconds)
                : lastDaysWithData.Average(d => (double)d.Reps);

            if (firstAvg == 0) return 0;
            return Math.Round(((lastAvg - firstAvg) / firstAvg) * 100, 1);
        }
        else
        {
            // Comparar primera semana vs última semana
            var firstWeek = dataPoints.FirstOrDefault();
            var lastWeek = dataPoints.LastOrDefault();

            if (firstWeek == null || lastWeek == null) return 0;

            var firstValue = isPlank ? firstWeek.Seconds : firstWeek.Reps;
            var lastValue = isPlank ? lastWeek.Seconds : lastWeek.Reps;

            if (firstValue == 0) return 0;
            return Math.Round(((double)(lastValue - firstValue) / firstValue) * 100, 1);
        }
    }

    private string GetConsistencyLabel(int daysWithActivity, string range)
    {
        if (range.ToLower() == "week")
        {
            if (daysWithActivity >= 6) return "Excelente";
            if (daysWithActivity >= 4) return "Buena";
            if (daysWithActivity >= 2) return "Regular";
            return "Baja";
        }
        else
        {
            // Para mes: evaluar sobre 28 días
            if (daysWithActivity >= 20) return "Excelente";
            if (daysWithActivity >= 12) return "Buena";
            if (daysWithActivity >= 6) return "Regular";
            return "Baja";
        }
    }

    private List<AspectScoreDto> GetAspectScores(List<TrainingSession> sessions, string exerciseType)
    {
        var aspectScores = new List<AspectScoreDto>();

        switch (exerciseType)
        {
            case "pushup":
                aspectScores.Add(CreateAspectScore("Postura", sessions, s => s.Metrics.ControlScore, "controlScore"));
                aspectScores.Add(CreateAspectScore("Velocidad", sessions, s => s.Metrics.RepsPerMinute, "repsPerMinute"));
                aspectScores.Add(CreateAspectScore("Rango", sessions, s => s.Metrics.DepthScore, "depthScore"));
                aspectScores.Add(CreateAspectScore("Estabilidad", sessions, s => s.Metrics.StabilityScore, "stabilityScore"));
                aspectScores.Add(CreateAspectScore("Consistencia", sessions, s => s.Metrics.ConsistencyScore, "consistencyScore"));
                break;

            case "squat":
                aspectScores.Add(CreateAspectScore("Postura", sessions, s => s.Metrics.AlignmentScore, "alignmentScore"));
                aspectScores.Add(CreateAspectScore("Velocidad", sessions, s => s.Metrics.RepsPerMinute, "repsPerMinute"));
                aspectScores.Add(CreateAspectScore("Rango", sessions, s => s.Metrics.DepthScore, "depthScore"));
                aspectScores.Add(CreateAspectScore("Estabilidad", sessions, s => s.Metrics.BalanceScore, "balanceScore"));
                aspectScores.Add(CreateAspectScore("Consistencia", sessions, s => s.Metrics.ConsistencyScore, "consistencyScore"));
                break;

            case "plank":
                aspectScores.Add(CreateAspectScore("Postura", sessions, s => s.Metrics.HipScore, "hipScore"));
                aspectScores.Add(CreateAspectScore("Estabilidad", sessions, s => s.Metrics.StabilityScore, "stabilityScore"));
                aspectScores.Add(CreateAspectScore("Core", sessions, s => s.Metrics.CoreScore, "coreScore"));
                aspectScores.Add(CreateAspectScore("Brazos", sessions, s => s.Metrics.ArmPositionScore, "armPositionScore"));
                aspectScores.Add(CreateAspectScore("Resistencia", sessions, s => s.Metrics.ResistanceScore, "resistanceScore"));
                break;
        }

        return aspectScores.Where(a => a.Score > 0).ToList();
    }

    private AspectScoreDto CreateAspectScore(string aspect, List<TrainingSession> sessions, Func<TrainingSession, double?> metricSelector, string metricName)
    {
        var values = sessions.Select(metricSelector).Where(v => v.HasValue && v.Value > 0).Select(v => v!.Value).ToList();
        var score = values.Any() ? Math.Round(values.Average(), 1) : 0;

        return new AspectScoreDto
        {
            Aspect = aspect,
            Score = score,
            Metric = metricName
        };
    }

    private int CalculateCurrentStreak(List<DateTime> sessionDates)
    {
        if (!sessionDates.Any()) return 0;

        var uniqueDates = sessionDates.Select(d => d.Date).Distinct().OrderByDescending(d => d).ToList();
        var today = DateTime.UtcNow.Date;
        
        // Si no hay sesión hoy ni ayer, la racha se rompió
        if (!uniqueDates.Contains(today) && !uniqueDates.Contains(today.AddDays(-1)))
        {
            return 0;
        }

        var streak = 0;
        var currentDate = uniqueDates.Contains(today) ? today : today.AddDays(-1);

        foreach (var date in uniqueDates)
        {
            if (date == currentDate)
            {
                streak++;
                currentDate = currentDate.AddDays(-1);
            }
            else if (date < currentDate)
            {
                break;
            }
        }

        return streak;
    }

    private int CalculateLongestStreak(List<DateTime> sessionDates)
    {
        if (!sessionDates.Any()) return 0;

        var uniqueDates = sessionDates.Select(d => d.Date).Distinct().OrderBy(d => d).ToList();
        var maxStreak = 1;
        var currentStreak = 1;

        for (int i = 1; i < uniqueDates.Count; i++)
        {
            if ((uniqueDates[i] - uniqueDates[i - 1]).Days == 1)
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
}

