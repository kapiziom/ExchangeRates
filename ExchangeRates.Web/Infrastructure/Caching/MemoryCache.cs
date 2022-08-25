using ExchangeRates.Common.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace ExchangeRates.Web.Infrastructure.Caching;

public class MemoryCache : ICache
{
    private readonly IMemoryCache _cache;

    public MemoryCache(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken ct = default)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = options.SlidingExpiration,
            AbsoluteExpiration = options.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow
        };

        _cache.Set(key, value, cacheEntryOptions);
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken ct = default) 
        where T : class => _cache.Get<T>(key);

    public async Task RemoveAsync(string key, CancellationToken ct = default) => _cache.Remove(key);
}