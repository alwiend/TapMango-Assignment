namespace SMSRateLimiter.API.Domain.Models;

public class SMSRateLimitResponse {
    public bool CanSend { get; set; }
    public string? Reason { get; set; }

    /// <summary>
    ///     Mapping function to more efficiently Map the tuple to the response object.
    /// </summary>
    /// <param name="data"></param>
    public static implicit operator SMSRateLimitResponse(Tuple<bool, string> data) {
        return new SMSRateLimitResponse() {
            CanSend = data.Item1,
            Reason = data.Item2,
        };
    }
}
