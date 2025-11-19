using System.ComponentModel.DataAnnotations;

namespace FitTrackerAPI.DTOs.Authentication;

public class RegisterDto
{
    [Required, EmailAddress]
    public string Email { get; set; } // Correo electrónico

    [Required]
    public string Password { get; set; }

    // Campos de UserProfile
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public string PhoneNumber { get; set; }

    [Required]
    public decimal Weight { get; set; } // Peso

    [Required]
    public decimal Height { get; set; } // Altura
}