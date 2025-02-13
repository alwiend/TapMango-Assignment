namespace SMSRateLimiter.API.APIs;

public static class SMSRateLimiterAPI {

    /// <summary>
    ///     Mapping the api group for the cansend endpoint
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static RouteGroupBuilder MapSMSRateLimiterAPI(this RouteGroupBuilder app) {
        app.MapGet("/cansend", CanSendSMSMessage);

        return app;
    }

    /// <summary>
    ///     a HTTP implementation to determine whether a SMS message can be sent for a given phone number and account.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="services"></param>
    /// <returns>The response object containing a field named "CanSend" which indicated whether a SMS can be sent or not, and the reason if "CanSend" is false.</returns>
    public static async Task<Results<Ok<SMSRateLimitResponse>, BadRequest<SMSRateLimitResponse>>> CanSendSMSMessage(HttpContext context, [AsParameters] SMSRateLimiterServices services) {
        try {
            var phoneNumber = context.Request.Query["phoneNumber"];
            var accountId = context.Request.Query["accountId"];

            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(phoneNumber)) {
                return TypedResults.Ok(new SMSRateLimitResponse() { CanSend = false, Reason = "Invalid Request" });
            }

            var canSendResponse = await services.CanSendSMSMessageAsync(phoneNumber!, accountId!);

            return TypedResults.Ok((SMSRateLimitResponse)canSendResponse);
        } catch (Exception ex) {
            services.Logger.LogError(ex, ex.Message);
            return TypedResults.BadRequest(new SMSRateLimitResponse() { CanSend = false, Reason = ex.Message });
        }
    }
}
