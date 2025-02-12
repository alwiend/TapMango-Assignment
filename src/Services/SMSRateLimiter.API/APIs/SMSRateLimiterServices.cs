using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace SMSRateLimiter.API.APIs;

public class SMSRateLimiterServices(ILogger<SMSRateLimiterServices> logger, IDistributedCache redisCache) {
    public ILogger<SMSRateLimiterServices> Logger { get; set; } = logger;
    public IDistributedCache RedisCache { get; set; } = redisCache;

    private const int PerNumberLimit = 5;
    private const int AccountLimit = 10;
    private const string NumberLimitKey = "sms_rate_limit:number:";
    private const string AccountLimitKey = "sms_rate_limit:account:";

    public async Task<Tuple<bool, string>> CanSendSMSMessageAsync(string phoneNumber, string accountId) {
        try {
            var phoneKey = NumberLimitKey + phoneNumber;
            var accountKey = AccountLimitKey + accountId;

            var phoneCount = await IncrementCacheValueAsync(RedisCache, phoneKey);
            var accountCount = await IncrementCacheValueAsync(RedisCache, accountKey);

            if (phoneCount > PerNumberLimit || accountCount > AccountLimit) {
                await DecrementCacheValueAsync(RedisCache, phoneKey);
                await DecrementCacheValueAsync(RedisCache, accountKey);
                return new Tuple<bool, string>(true, "Rate limit exceeded");
            }

            return new Tuple<bool, string>(true, string.Empty);
        } catch (Exception ex) {
            logger.LogError(ex, $"An error occured: {ex.Message}");
            return new Tuple<bool, string>(false, ex.Message);
        }
    }

    private async Task<int> IncrementCacheValueAsync(IDistributedCache cache, string key) {
        var value = await cache.GetStringAsync(key);
        var count = string.IsNullOrEmpty(value) ? 0 : int.Parse(value);
        count++;
        await cache.SetStringAsync(key, count.ToString(), new DistributedCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1)
        });
        return count;
    }

    private async Task DecrementCacheValueAsync(IDistributedCache cache, string key) {
        var value = await cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(value)) return;
        var count = int.Parse(value);
        count--;
        await cache.SetStringAsync(key, count.ToString(), new DistributedCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1)
        });
    }
}
