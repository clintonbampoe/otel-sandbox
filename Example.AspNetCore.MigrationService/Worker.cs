using System.Diagnostics;
using Example.AspNetCore.Data;
using Example.AspNetCore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Example.AspNetCore.MigrationService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime
) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource SActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var activity = SActivitySource.StartActivity(
            "Migrating Database",
            ActivityKind.Client
        );

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await RunMigrationAsync(dbContext, ct);
            await SeedDataAsync(dbContext, ct);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(AppDbContext dbContext, CancellationToken ct)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            await dbContext.Database.MigrateAsync(ct);
        });
    }

    private static async Task SeedDataAsync(AppDbContext dbContext, CancellationToken ct)
    {
        var firstTodo = new Todo
        {
            Title = "First Task",
            IsCompleted = true,
            CreatedAt = DateTime.UtcNow,
        };

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

            await dbContext.Todos.AddAsync(firstTodo, ct);
            await dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        });
    }
}
