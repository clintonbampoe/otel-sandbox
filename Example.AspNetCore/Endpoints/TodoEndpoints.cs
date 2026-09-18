using System.Diagnostics;
using Example.AspNetCore.Data.Models;
using Example.AspNetCore.Services;

namespace Example.AspNetCore.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/hello", Hello);
        app.MapPost("/todos", Create);
        app.MapGet("/todos/{id:int}", Get);
        app.MapGet("/todos", List);
        app.MapPut("/todos/{id:int}", Update);
    }

    private static IResult Hello(ILogger<Todo> logger, InstrumentationSource instrumentation)
    {
        using var activity = instrumentation.ActivitySource.StartActivity();
        logger.LogInformation("Said Greetings.");
        return Results.Ok("Hello, World!");
    }

    private static async Task<IResult> Create(
        TodoDto dto,
        TodoService service,
        ILogger<Todo> logger,
        InstrumentationSource instrumentation,
        CancellationToken ct = default
    )
    {
        using var activity = instrumentation.ActivitySource.StartActivity("CreateTodo");
        Log.TodoCreateRequest(logger, dto.Title);
        var todo = await service.Create(dto, ct);
        Log.TodoCreateReturn201(logger, todo.Id);
        return Results.Created($"/todos/{todo.Id}", todo);
    }

    private static async Task<IResult> Update(
        int id,
        TodoDto dto,
        TodoService service,
        InstrumentationSource instrumentation,
        ILogger<Todo> logger,
        CancellationToken ct = default
    )
    {
        using var activity = instrumentation.ActivitySource.StartActivity("UpdateTodo");
        activity?.SetTag("todo.id", id);
        Log.TodoUpdateRequest(logger, dto.Title);
        var todo = await service.Update(id, dto, ct);
        if (todo is null)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Todo not found");
            return Results.NotFound();
        }
        return Results.Ok(todo);
    }

    private static async Task<IResult> List(
        TodoService service,
        InstrumentationSource instrumentation,
        ILogger<Todo> logger,
        CancellationToken ct = default
    )
    {
        using var activity = instrumentation.ActivitySource.StartActivity("ListTodos");
        Log.TodoGetListRequest(logger);
        var todos = await service.List(ct);
        return Results.Ok(todos);
    }

    private static async Task<IResult> Get(
        int id,
        TodoService service,
        InstrumentationSource instrumentation,
        ILogger<Todo> logger,
        CancellationToken ct = default
    )
    {
        using var activity = instrumentation.ActivitySource.StartActivity("GetTodo");
        activity?.SetTag("todo.id", id);
        Log.TodoGetByIdRequest(logger, id);
        var todo = await service.GetById(id, ct);
        if (todo is null)
        {
            activity?.SetStatus(ActivityStatusCode.Error, "Todo not found");
            return Results.NotFound();
        }
        return Results.Ok(todo);
    }
}

public static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Todo create request received. Title: {title}"
    )]
    public static partial void TodoCreateRequest(ILogger logger, string title);

    [LoggerMessage(Level = LogLevel.Information, Message = "Return 201 for todo. TodoId {TodoId}")]
    public static partial void TodoCreateReturn201(ILogger logger, int todoId);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Todo update request received. Title: {title}"
    )]
    public static partial void TodoUpdateRequest(ILogger logger, string title);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Todo get request received for id {id}")]
    public static partial void TodoGetByIdRequest(ILogger logger, int id);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Todo get list request received.")]
    public static partial void TodoGetListRequest(ILogger logger);
}
