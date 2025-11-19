using FitTrackerAPI.Models.Exercises;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FitTrackerAPI.Repositories.Exercises;

public class ExerciseRepository : IExerciseRepository
{
    private readonly IMongoCollection<Exercise> _exercisesCollection;

    public ExerciseRepository(IMongoDatabase database)
    {
        _exercisesCollection = database.GetCollection<Exercise>("Exercises");
    }

    public async Task<List<Exercise>> GetAllAsync() =>
        await _exercisesCollection.Find(_ => true).ToListAsync();

    public async Task<Exercise?> GetByIdAsync(string id) =>
        await _exercisesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Exercise newExercise) =>
        await _exercisesCollection.InsertOneAsync(newExercise);
}