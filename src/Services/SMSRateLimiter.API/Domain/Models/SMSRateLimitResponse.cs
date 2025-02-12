namespace SMSRateLimiter.API.Domain.Models;

public class SMSRateLimitResponse {
    public bool CanSend { get; set; }
    public string? Reason { get; set; }
}
