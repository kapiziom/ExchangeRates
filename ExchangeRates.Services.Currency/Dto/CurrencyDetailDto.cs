using ExchangeRates.Data.Entities;

namespace ExchangeRates.Services.Currency.Dto;

public class CurrencyDetailDto
{
    public CurrencyDetailDto(CurrencyEntity entity)
    {
        Id = entity.Id;
        Code = entity.Code;
        Name = entity.Name;
        Provider = entity.Provider;
        EffectiveDate = entity.EffectiveDate;
        Rates = entity.Rates.Select(o => new CurrencyRateDto(o));
    }
    
    public int Id { get; set; }
    
    public string Code { get; set; }
    
    public string Name { get; set; }
    
    public string? Provider { get; set; }
    
    public DateTime? EffectiveDate { get; set; }
    
    public IEnumerable<CurrencyRateDto> Rates { get; set; }
}