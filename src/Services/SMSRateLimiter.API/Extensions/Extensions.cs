namespace SMSRateLimiter.API.Extensions;

public static class Extensions {
    public static void AddApplicationServices(this IHostApplicationBuilder builder) {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAntiforgery();

        builder.AddRedisClient(connectionName: "cache");

        builder.Services.AddScoped<SMSRateLimiterServices>();
    }
}
