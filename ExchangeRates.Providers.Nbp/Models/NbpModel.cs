using System.Text.Json.Serialization;

namespace ExchangeRates.Providers.Nbp.Models;

public class NbpModel
{
    [JsonPropertyName("no")]
    public string TableNumber { get; set; }
    
    [JsonPropertyName("effectiveDate")]
    public DateTime EffectiveDate { get; set; }
    
    [JsonPropertyName("rates")]
    public IEnumerable<NbpCurrencyRate> Rates { get; set; }
}