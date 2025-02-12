var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache");

builder.AddProject<Projects.SMSRateLimiter_API>("smsratelimiter-api")
    .WithReference(redis);

builder.Build().Run();
