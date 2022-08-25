namespace ExchangeRates.Common.Caching;

public interface ICache
{
    Task SetAsync<T>(string key, T value, CacheEntryOptions options, CancellationToken ct = default);

    Task<T> GetAsync<T>(string key, CancellationToken ct = default) where T : class;

    Task RemoveAsync(string key, CancellationToken ct = default);
}