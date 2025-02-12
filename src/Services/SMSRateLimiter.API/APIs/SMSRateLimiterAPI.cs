namespace SMSRateLimiter.API.APIs;

public static class SMSRateLimiterAPI {

    public static RouteGroupBuilder MapSMSRateLimiterAPI(this RouteGroupBuilder app) {
        app.MapGet("/cansend", CanSendSMSMessage);

        return app;
    }

    public static async Task<Results<Ok<SMSRateLimitResponse>, BadRequest<SMSRateLimitResponse>>> CanSendSMSMessage([FromBody] GetSMSCanSendRequest request, [AsParameters] SMSRateLimiterServices services) {
        try {
            if (request == null || string.IsNullOrEmpty(request.PhoneNumber)) {
                throw new Exception("Invalid request");
            }

            SMSRateLimitResponse response = new() {
                CanSend = true
            };
            return TypedResults.Ok(response);
        } catch (Exception ex) {
            return TypedResults.BadRequest(new SMSRateLimitResponse() { CanSend = false, Reason = ex.Message });
        }
    }
}
