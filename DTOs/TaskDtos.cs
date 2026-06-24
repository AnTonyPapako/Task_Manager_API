using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.DTOs;

// Χρησιμοποιείται στο POST /tasks
public class TaskCreateDto
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Status { get; set; } = "pending";
}

// Χρησιμοποιείται στο PUT /tasks/{id} — όλα προαιρετικά (partial update)
public class TaskUpdateDto
{
    [MaxLength(255)]
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }
}

// Αυτό επιστρέφεται στον client
public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
