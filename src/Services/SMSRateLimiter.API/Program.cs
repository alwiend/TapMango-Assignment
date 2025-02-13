var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGroup("api/v1/SMSRate")
    .WithTags("SMS Rate Limiter API")
    .MapSMSRateLimiterAPI();

app.UseHttpsRedirection();

app.Run();