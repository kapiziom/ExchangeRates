namespace ExchangeRates.Services.Currency.Providers;

public class CurrencyProviderDto
{
    public string Provider { get; set; }
    
    public string Description { get; set; }
    
    public DateTime EffectiveDate { get; set; }

    public IEnumerable<CurrencyRate> Rates { get; set; }
}