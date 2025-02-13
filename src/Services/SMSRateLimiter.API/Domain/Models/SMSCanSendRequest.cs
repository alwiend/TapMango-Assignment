namespace SMSRateLimiter.API.Domain.Models;

public class SMSCanSendRequest {
    public string PhoneNumber { get; set; }
    public string AccountId { get; set; }
}
