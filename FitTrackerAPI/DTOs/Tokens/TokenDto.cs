namespace FitTrackerAPI.DTOs.Tokens;

public class TokenDto
{
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
    public string RefreshToken { get; set; }
    public bool IsVerified { get; set; }
}