namespace ExchangeRates.Services.Currency.Providers;

public interface ICurrencyProvider
{
    Task<CurrencyProviderDto> GetRatesAsync(string currencyCode, CancellationToken ct = default);
}