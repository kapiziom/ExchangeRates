using System.Text.Json.Serialization;

namespace ExchangeRates.Providers.Nbp.Models;

public class NbpCurrencyRate
{
    [JsonPropertyName("code")]
    public string Code { get; set; }
    
    [JsonPropertyName("currency")]
    public string Name { get; set; }

    [JsonPropertyName("mid")]
    public decimal Rate { get; set; }
}