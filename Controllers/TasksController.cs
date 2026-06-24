using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.DTOs;
using TaskManagerApi.Models;

namespace TaskManagerApi.Controllers;

[ApiController]
[Route("tasks")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    private static TaskResponseDto ToDto(TaskItem t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Status = t.Status,
        CreatedAt = t.CreatedAt,
    };

    // POST /tasks
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] TaskCreateDto dto)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status ?? "pending",
            CreatedAt = DateTime.UtcNow,
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { taskId = task.Id }, ToDto(task));
    }

    // GET /tasks?skip=0&limit=100
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasks(int skip = 0, int limit = 100)
    {
        var tasks = await _context.Tasks
            .OrderBy(t => t.Id)
            .Skip(skip)
            .Take(limit)
            .ToListAsync();

        return Ok(tasks.Select(ToDto));
    }

    // GET /tasks/{taskId}
    [HttpGet("{taskId:int}")]
    public async Task<ActionResult<TaskResponseDto>> GetTask(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task is null)
        {
            return NotFound(new { detail = "Task not found" });
        }

        return Ok(ToDto(task));
    }

    // PUT /tasks/{taskId}
    [HttpPut("{taskId:int}")]
    public async Task<ActionResult<TaskResponseDto>> UpdateTask(int taskId, [FromBody] TaskUpdateDto dto)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task is null)
        {
            return NotFound(new { detail = "Task not found" });
        }

        // Partial update: αλλάζουμε μόνο τα πεδία που στάλθηκαν (όχι null)
        if (dto.Title is not null) task.Title = dto.Title;
        if (dto.Description is not null) task.Description = dto.Description;
        if (dto.Status is not null) task.Status = dto.Status;

        await _context.SaveChangesAsync();

        return Ok(ToDto(task));
    }

    // DELETE /tasks/{taskId}
    [HttpDelete("{taskId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task is null)
        {
            return NotFound(new { detail = "Task not found" });
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
