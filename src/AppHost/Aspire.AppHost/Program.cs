var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SMSRateLimiter_API>("smsratelimiter-api");

builder.Build().Run();
