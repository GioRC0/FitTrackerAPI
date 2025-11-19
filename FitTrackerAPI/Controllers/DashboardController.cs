using System.Security.Claims;
using FitTrackerAPI.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitTrackerAPI.Controllers;

[ApiController]
[Route("api/mainDashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Obtener resumen del dashboard de inicio
    /// </summary>
    [HttpGet("home")]
    public async Task<IActionResult> GetHomeDashboard()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuario no autenticado" });
            }

            var dashboard = await _dashboardService.GetHomeDashboardAsync(userId);

            return Ok(new { success = true, data = dashboard });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener dashboard", error = ex.Message });
        }
    }
}

