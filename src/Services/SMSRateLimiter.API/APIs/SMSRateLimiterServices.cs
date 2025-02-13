namespace SMSRateLimiter.API.APIs;

public class SMSRateLimiterServices(ILogger<SMSRateLimiterServices> logger, IConnectionMultiplexer redis) {
    public ILogger<SMSRateLimiterServices> Logger { get; set; } = logger;
    public IConnectionMultiplexer Redis { get; set; } = redis;

    private const int PerNumberLimit = 5; // Improvement here -> Have configurable rate limit for phone numbers
    private const int AccountLimit = 10; // Improvement here -> Have configurable rate limit for accounts
    private const int NumberExpireInSeconds = 1; // Improvement here -> same as above
    private const int AccountExpireInSeconds = 1; // Improvement here -> same as above
    private const string NumberLimitKey = "sms_rate_limit:number:";
    private const string AccountLimitKey = "sms_rate_limit:account:";

    /// <summary>
    ///     Async method that will return a tuple containing either true or false on whether an SMS can be sent.
    ///     Uses redis cache in order to track the number of messages that have been sent by both an account and a phone number.
    ///     The account and phone number data will only persist in the cache until it is expired. 
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="accountId"></param>
    /// <returns></returns>
    public async Task<Tuple<bool, string>> CanSendSMSMessageAsync(string phoneNumber, string accountId) {
        try {
            var redisCache = Redis.GetDatabase();

            var phoneKey = NumberLimitKey + phoneNumber;
            var accountKey = AccountLimitKey + accountId;

            var phoneCount = await redisCache.StringIncrementAsync(phoneKey);
            var accountCount = await redisCache.StringIncrementAsync(accountKey);

            if (phoneCount == 1) { 
                await redisCache.KeyExpireAsync(phoneKey, TimeSpan.FromSeconds(NumberExpireInSeconds)); 
            }
            if (accountCount == 1) {
                await redisCache.KeyExpireAsync(accountKey, TimeSpan.FromSeconds(AccountExpireInSeconds));
            }

            // If either of the phone number or account have reached their limit then decrement each and return false
            if (phoneCount > PerNumberLimit || accountCount > AccountLimit) {
                await redisCache.StringDecrementAsync(phoneKey);
                await redisCache.StringDecrementAsync(accountKey);
                return new Tuple<bool, string>(false, "Rate limit exceeded");
            }

            return new Tuple<bool, string>(true, string.Empty);
        } catch (Exception ex) {
            logger.LogError(ex, $"An error occured: {ex.Message}");
            return new Tuple<bool, string>(false, ex.Message);
        }
    }
}
