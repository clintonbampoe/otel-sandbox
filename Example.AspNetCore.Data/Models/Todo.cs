using System.ComponentModel.DataAnnotations;

namespace Example.AspNetCore.Data.Models;

public class Todo
{
    public int Id { get; set; }

    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
