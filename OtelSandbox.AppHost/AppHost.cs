using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var todoDb = builder.AddPostgres("postgres").WithDataVolume().AddDatabase("todoDb");

var migrations = builder
    .AddProject<Example_AspNetCore_MigrationService>("migrations")
    .WithReference(todoDb)
    .WaitFor(todoDb);

builder
    .AddProject<Example_AspNetCore>("api")
    .WithReference(todoDb)
    .WaitFor(todoDb)
    .WaitForCompletion(migrations);

builder.Build().Run();
