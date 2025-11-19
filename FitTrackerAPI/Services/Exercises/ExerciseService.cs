using FitTrackerAPI.DTOs.Exercises;
using FitTrackerAPI.Models.Exercises;
using FitTrackerAPI.Repositories.Exercises;

namespace FitTrackerAPI.Services.Exercises;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _exerciseRepository;

    public ExerciseService(IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<List<Exercise>> GetAllAsync() =>
        await _exerciseRepository.GetAllAsync();

    public async Task<Exercise> CreateAsync(CreateExerciseDto createExerciseDto)
    {
        var newExercise = new Exercise
        {
            Name = createExerciseDto.Name,
            ShortDescription = createExerciseDto.ShortDescription,
            FullDescription = createExerciseDto.FullDescription,
            Difficulty = createExerciseDto.Difficulty,
            MuscleGroup = createExerciseDto.MuscleGroup,
            MinTime = createExerciseDto.MinTime,
            MaxTime = createExerciseDto.MaxTime,
            Steps = createExerciseDto.Steps,
            Tips = createExerciseDto.Tips,
            ImageUrl = createExerciseDto.ImageUrl,
            ShortImageUrl = createExerciseDto.ShortImageUrl
        };

        await _exerciseRepository.CreateAsync(newExercise);
        return newExercise;
    }
}