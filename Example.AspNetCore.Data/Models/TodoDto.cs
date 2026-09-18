namespace Example.AspNetCore.Data.Models;

public record TodoDto
{
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
};
