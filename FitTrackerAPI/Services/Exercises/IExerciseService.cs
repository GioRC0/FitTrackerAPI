using FitTrackerAPI.DTOs.Exercises;
using FitTrackerAPI.Models.Exercises;

namespace FitTrackerAPI.Services.Exercises;

public interface IExerciseService
{
    Task<List<Exercise>> GetAllAsync();
    Task<Exercise> CreateAsync(CreateExerciseDto createExerciseDto);
}