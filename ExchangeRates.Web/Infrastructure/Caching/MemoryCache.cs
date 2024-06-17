using ExchangeRates.Common.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace ExchangeRates.Web.Infrastructure.Caching;

public class MemoryCache(IMemoryCache cache) : ICache
{
    public async Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken ct = default)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = options.SlidingExpiration,
            AbsoluteExpiration = options.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow
        };

        cache.Set(key, value, cacheEntryOptions);
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken ct = default) 
        where T : class => cache.Get<T>(key);

    public async Task RemoveAsync(string key, CancellationToken ct = default) => cache.Remove(key);
}