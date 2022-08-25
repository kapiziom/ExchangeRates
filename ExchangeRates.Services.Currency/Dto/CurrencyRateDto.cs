using ExchangeRates.Data.Entities;

namespace ExchangeRates.Services.Currency.Dto;

public class CurrencyRateDto
{
    public CurrencyRateDto(CurrencyRateEntity entity)
    {
        Symbol = entity.FromCurrency.Code;
        Name = entity.FromCurrency.Name;
        Rate = entity.Rate;
        UpdatedAt = entity.UpdatedAt;
    }
    
    public string Symbol { get; set; }
    
    public string Name { get; set; }
    
    public decimal Rate { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}