using System.Security.Claims;
using FitTrackerAPI.DTOs.UserProfile;
using FitTrackerAPI.Services.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitTrackerAPI.Controllers;

[ApiController]
[Route("api/users/profile")]
[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;

    public UserProfileController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    /// <summary>
    /// Obtener información completa del perfil del usuario autenticado
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var profile = await _userProfileService.GetUserProfileAsync(userId);
            
            return Ok(new { success = true, data = profile });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener perfil", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener estadísticas del usuario
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var stats = await _userProfileService.GetUserStatsAsync(userId);
            
            return Ok(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener estadísticas", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtener logros/achievements del usuario
    /// </summary>
    [HttpGet("achievements")]
    public async Task<IActionResult> GetAchievements()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var achievements = await _userProfileService.GetUserAchievementsAsync(userId);
            
            return Ok(new { success = true, data = achievements });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener logros", error = ex.Message });
        }
    }

    /// <summary>
    /// Actualizar información del perfil
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var profile = await _userProfileService.UpdateUserProfileAsync(userId, request);
            
            return Ok(new 
            { 
                success = true, 
                message = "Perfil actualizado correctamente",
                data = profile 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar perfil", error = ex.Message });
        }
    }
}

