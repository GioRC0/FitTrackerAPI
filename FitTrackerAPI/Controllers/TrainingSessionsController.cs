using System.Security.Claims;
using FitTrackerAPI.DTOs.Training;
using FitTrackerAPI.Services.Training;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitTrackerAPI.Controllers;

[ApiController]
[Route("api/trainingSessions")]
[Authorize]
public class TrainingSessionsController : ControllerBase
{
    private readonly ITrainingSessionService _trainingSessionService;

    public TrainingSessionsController(ITrainingSessionService trainingSessionService)
    {
        _trainingSessionService = trainingSessionService;
    }

    /// <summary>
    /// Crear una nueva sesión de entrenamiento
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateSession([FromBody] TrainingSessionCreateDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var session = await _trainingSessionService.CreateSessionAsync(dto, userId);
            
            return CreatedAtAction(
                nameof(GetSessionById), 
                new { id = session.Id }, 
                new { message = "Sesión de entrenamiento creada exitosamente", data = session }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al crear la sesión de entrenamiento", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener una sesión de entrenamiento por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSessionById(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var session = await _trainingSessionService.GetSessionByIdAsync(id, userId);
            
            if (session == null)
            {
                return NotFound(new { message = "Sesión de entrenamiento no encontrada o no pertenece al usuario" });
            }

            return Ok(new { data = session });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener la sesión de entrenamiento", error = ex.Message });
        }
    }

    /// <summary>
    /// Listar sesiones de entrenamiento del usuario autenticado
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetUserSessions(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] string? exerciseId = null)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var sessions = await _trainingSessionService.GetUserSessionsAsync(userId, page, pageSize, exerciseId);
            
            return Ok(new 
            { 
                data = sessions,
                pagination = new
                {
                    page,
                    pageSize,
                    total = sessions.Count
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener las sesiones de entrenamiento", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener progreso semanal de un ejercicio
    /// </summary>
    [HttpGet("weekly-progress/{exerciseId}")]
    public async Task<IActionResult> GetWeeklyProgress(string exerciseId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var progress = await _trainingSessionService.GetWeeklyProgressAsync(userId, exerciseId);
            
            return Ok(new { data = progress });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener el progreso semanal", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener estadísticas completas de un ejercicio (resumen semanal + sesiones recientes)
    /// </summary>
    [HttpGet("exercise/{exerciseId}/stats")]
    public async Task<IActionResult> GetExerciseStats(string exerciseId, [FromQuery] int recentLimit = 5)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            if (recentLimit < 1 || recentLimit > 50) recentLimit = 5;

            var stats = await _trainingSessionService.GetExerciseStatsAsync(userId, exerciseId, recentLimit);
            
            return Ok(new { data = stats });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener estadísticas del ejercicio", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener datos de progreso por período (semana o mes)
    /// </summary>
    [HttpGet("exercise/{exerciseId}/progress")]
    public async Task<IActionResult> GetProgressData(string exerciseId, [FromQuery] string range = "week")
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            // Validar range
            range = range.ToLower();
            if (range != "week" && range != "month")
            {
                return BadRequest(new { message = "El parámetro 'range' debe ser 'week' o 'month'" });
            }

            var progressData = await _trainingSessionService.GetProgressDataAsync(userId, exerciseId, range);
            
            return Ok(new { success = true, data = progressData });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener datos de progreso", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener análisis de técnica detallado
    /// </summary>
    [HttpGet("exercise/{exerciseId}/form-analysis")]
    public async Task<IActionResult> GetFormAnalysis(string exerciseId, [FromQuery] string range = "week")
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            // Validar range
            range = range.ToLower();
            if (range != "week" && range != "month")
            {
                return BadRequest(new { message = "El parámetro 'range' debe ser 'week' o 'month'" });
            }

            var formAnalysis = await _trainingSessionService.GetFormAnalysisAsync(userId, exerciseId, range);
            
            return Ok(new { success = true, data = formAnalysis });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener análisis de técnica", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener objetivos y metas del usuario para un ejercicio
    /// </summary>
    [HttpGet("exercise/{exerciseId}/goals")]
    public async Task<IActionResult> GetGoals(string exerciseId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var goals = await _trainingSessionService.GetGoalsAsync(userId, exerciseId);
            
            return Ok(new { success = true, data = goals });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener objetivos", error = ex.Message });
        }
    }
}

