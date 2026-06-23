var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Kutxa_Presentation>("kutxa-presentation");

builder.Build().Run();
