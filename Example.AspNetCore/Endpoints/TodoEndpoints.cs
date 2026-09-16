using Example.AspNetCore.Data;
using Example.AspNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Example.AspNetCore.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoRoutes(this IEndpointRouteBuilder app)
    {
        app.MapPost("/todos", Create);
        app.MapGet("/todos/{id:int}", Get);
        app.MapGet("/todos", List);
        app.MapGet("/hello", Hello);
    }

    private static IResult Hello(ILogger<Todo> logger)
    {
        logger.LogInformation("Said Greetings.");
        return Results.Ok("Hello!");
    }

    private static async Task<IResult> Create(
        TodoDto dto,
        TodoDb db,
        InstrumentationSource instrumentation,
        ILogger<Todo> logger,
        CancellationToken ct = default
    )
    {
        using var activity = instrumentation.ActivitySource.StartActivity();

        var todo = new Todo
        {
            Title = dto.Title,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
        };

        db.Todos.Add(todo);
        await db.SaveChangesAsync(ct);

        instrumentation.TodosCreatedCounter.Add(1);
        activity?.SetTag("todo.id", todo.Id);
        activity?.SetTag("todo.title", todo.Title);

        logger.LogInformation("Todo created {TodoId}: {Title}", todo.Id, todo.Title);

        return Results.Created($"/todos/{todo.Id}", todo);
    }

    private static async Task<IResult> List(
        TodoDb db,
        ILogger<Todo> logger,
        CancellationToken ct = default
    )
    {
        var todos = await db.Todos.ToListAsync(ct);
        logger.LogInformation("Fetched all todos. Count: {Count}", todos.Count);
        return Results.Ok(todos);
    }

    private static async Task<IResult> Get(
        int id,
        TodoDb db,
        ILogger<Todo> logger,
        CancellationToken ct = default
    )
    {
        var todo = await db.Todos.FindAsync(id, ct);
        if (todo is null)
        {
            logger.LogWarning("Todo {TodoId} not found.", id);
            return Results.NotFound();
        }

        logger.LogInformation("Fetched todo {TodoId}", id);
        return Results.Ok(todo);
    }
}
