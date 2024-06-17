using System.Text.Json;
using ExchangeRates.Providers.Nbp.Exceptions;
using ExchangeRates.Providers.Nbp.Models;
using ExchangeRates.Services.Currency.Providers;

namespace ExchangeRates.Providers.Nbp;

public class NbpProvider(IHttpClientFactory clientFactory) : ICurrencyProvider
{
    private const string BaseCurrency = "PLN";
    
    private readonly HttpClient _httpClient = clientFactory.CreateClient();

    public async Task<CurrencyProviderDto> GetRatesAsync(string currencyCode, CancellationToken ct = default)
    {
        if (currencyCode is not BaseCurrency)
            throw new CurrencyCodeMismatch(currencyCode);
        
        var nbpModel = await GetNbpResponseAsync(ct);

        return new CurrencyProviderDto
        {
            Provider = "NBP",
            Description = nbpModel.TableNumber,
            EffectiveDate = nbpModel.EffectiveDate,
            Rates = nbpModel.Rates.Select(o => new CurrencyRate
            {
                Code = o.Code,
                Name = o.Name,
                Rate = o.Rate
            })
        };
    }
    
    private async Task<NbpModel> GetNbpResponseAsync(CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, 
            "https://api.nbp.pl/api/exchangerates/tables/a?format=json");
        
        var response = await _httpClient.SendAsync(request, ct);
            
        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to fetch exchange rates. Returned {response.StatusCode} status code.");
        
        var responseString = await response.Content.ReadAsStringAsync(ct);

        var model = JsonSerializer.Deserialize<IEnumerable<NbpModel>>(responseString)?.FirstOrDefault();
        
        if (model is null)
            throw new Exception("Failed to map nbp model.");

        return model;
    }
}