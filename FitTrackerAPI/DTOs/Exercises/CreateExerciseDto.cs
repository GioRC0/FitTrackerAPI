using System.ComponentModel.DataAnnotations;

namespace FitTrackerAPI.DTOs.Exercises;

public class CreateExerciseDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string ShortDescription { get; set; }

    [Required]
    public string FullDescription { get; set; }

    [Required]
    public string Difficulty { get; set; }

    [Required]
    public string MuscleGroup { get; set; }

    public int MinTime { get; set; }
    public int MaxTime { get; set; }

    public List<string> Steps { get; set; } = new List<string>();
    public List<string> Tips { get; set; } = new List<string>();

    [Required]
    public string ImageUrl { get; set; }

    public string ShortImageUrl { get; set; }
}