using System.ComponentModel.DataAnnotations;

namespace FitTrackerAPI.DTOs.Tokens;

public class VerifyCodeDto
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Code { get; set; }
}