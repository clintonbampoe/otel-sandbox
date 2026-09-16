using Projects;

var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Example_AspNetCore>("api");
builder.Build().Run();
