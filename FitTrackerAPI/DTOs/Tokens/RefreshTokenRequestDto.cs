using System.ComponentModel.DataAnnotations;

namespace FitTrackerAPI.DTOs.Tokens;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; }
}