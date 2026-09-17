using System.Diagnostics;
using Example.AspNetCore.Data;
using Example.AspNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Example.AspNetCore.Services;

public class TodoService(TodoDb db, ILogger<Todo> logger, InstrumentationSource instrumentation)
{
    public async Task<List<Todo>> List(CancellationToken ct = default)
    {
        using var activity = instrumentation.ActivitySource.StartActivity();
        var todos = await db.Todos.AsNoTracking().ToListAsync(ct);
        Log.TodoListFetched(logger);
        return todos;
    }

    public async Task<Todo?> GetById(int id, CancellationToken ct = default)
    {
        using var activity = instrumentation.ActivitySource.StartActivity();
        var todo = await db.Todos.FindAsync(id, ct);

        if (todo is null)
        {
            Log.TodoNotFound(logger, id);
            return todo;
        }

        Log.TodoFetched(logger, id);
        return todo;
    }

    public async Task<Todo> Create(TodoDto dto, CancellationToken ct = default)
    {
        using var activity = instrumentation.ActivitySource.StartActivity();
        var todo = new Todo
        {
            Title = dto.Title,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
        };

        var sw = Stopwatch.StartNew();
        db.Todos.Add(todo);
        await db.SaveChangesAsync(ct);
        instrumentation.CreateDuration.Record(sw.ElapsedMilliseconds);
        instrumentation.TodosCreatedCounter.Add(1);
        Log.TodoCreated(logger, todo.Id);
        return todo;
    }

    public async Task<Todo?> Update(int id, TodoDto dto, CancellationToken ct = default)
    {
        using var activity = instrumentation.ActivitySource.StartActivity();
        var todo = await db.Todos.FindAsync(id, ct);

        if (todo is null)
        {
            Log.TodoNotFound(logger, id);
            return todo;
        }

        todo.Title = dto.Title;
        todo.IsCompleted = dto.IsCompleted;
        await db.SaveChangesAsync(ct);

        Log.TodoUpdated(logger, todo.Id);
        return todo;
    }
}

public static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Todo created. TodoId: {TodoId}")]
    public static partial void TodoCreated(ILogger logger, int todoId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Todo updated. TodoId {TodoId}")]
    public static partial void TodoUpdated(ILogger logger, int todoId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Todo fetched. TodoId {TodoId}")]
    public static partial void TodoFetched(ILogger logger, int todoId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Todo list fetched.")]
    public static partial void TodoListFetched(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Todo not found. TodoId {TodoId}")]
    public static partial void TodoNotFound(ILogger logger, int todoId);
}
