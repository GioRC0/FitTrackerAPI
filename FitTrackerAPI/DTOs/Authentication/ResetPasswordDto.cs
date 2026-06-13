using System.ComponentModel.DataAnnotations;

namespace FitTrackerAPI.DTOs.Authentication;

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; }

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; }
}
