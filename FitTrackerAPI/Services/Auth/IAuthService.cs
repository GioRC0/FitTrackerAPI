using FitTrackerAPI.DTOs.Authentication;
using FitTrackerAPI.DTOs.Tokens;
using FitTrackerAPI.Models.UserInfo;

namespace FitTrackerAPI.Services.Auth;

public interface IAuthService
{
    // Autenticación
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
    
    // Registro y Perfil (Requisito 3)
    Task<User> RegisterAsync(RegisterDto dto);
    
    // Tokens (Requisito 1)
    Task<TokenDto> GenerateTokensAsync(User user);
    
    // Refresh Token (Requisito 5)
    Task<TokenDto> RefreshTokenAsync(string refreshToken);

    // Verificación de Email (Requisito 4)
    Task SendVerificationCodeAsync(User user);
    Task<bool> VerifyCodeAsync(VerifyCodeDto dto);
}