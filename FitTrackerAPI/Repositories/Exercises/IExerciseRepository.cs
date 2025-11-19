using FitTrackerAPI.Models.Exercises;

namespace FitTrackerAPI.Repositories.Exercises;

public interface IExerciseRepository
{
    Task<List<Exercise>> GetAllAsync();
    Task<Exercise?> GetByIdAsync(string id);
    Task CreateAsync(Exercise newExercise);
}