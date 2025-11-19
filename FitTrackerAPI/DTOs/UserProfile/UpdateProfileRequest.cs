namespace FitTrackerAPI.DTOs.UserProfile;

public class UpdateProfileRequest
{
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public double Weight { get; set; }
    public int Height { get; set; }
}

