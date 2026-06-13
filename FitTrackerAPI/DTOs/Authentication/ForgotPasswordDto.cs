using System.ComponentModel.DataAnnotations;

namespace FitTrackerAPI.DTOs.Authentication;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
