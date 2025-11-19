using FitTrackerAPI.DTOs.Exercises;
using FitTrackerAPI.Models.Exercises;
using FitTrackerAPI.Services.Exercises;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitTrackerAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/exercises")]
public class ExercisesController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExercisesController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Exercise>>> Get() =>
        await _exerciseService.GetAllAsync();

    [HttpPost]
    public async Task<IActionResult> Post(CreateExerciseDto createExerciseDto)
    {
        var createdExercise = await _exerciseService.CreateAsync(createExerciseDto);

        return CreatedAtAction(nameof(Get), new { id = createdExercise.Id }, createdExercise);
    }
}