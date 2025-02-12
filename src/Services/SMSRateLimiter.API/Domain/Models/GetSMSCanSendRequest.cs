namespace SMSRateLimiter.API.Domain.Models;

public class GetSMSCanSendRequest {
    public string PhoneNumber { get; set; }
    public string AccountId { get; set; }
}
