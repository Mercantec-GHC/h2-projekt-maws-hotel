var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.API>("api");

builder.AddProject<Projects.Blazor>("blazor");

builder.Build().Run();

