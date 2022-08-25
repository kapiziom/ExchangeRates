using ExchangeRates.Data.Entities;

namespace ExchangeRates.Services.Currency.Dto;

public class CurrencyDto
{
    public CurrencyDto(CurrencyEntity entity)
    {
        Id = entity.Id;
        Code = entity.Code;
        Name = entity.Name;
    }
    
    public int Id { get; set; }
    
    public string Code { get; set; }
    
    public string Name { get; set; }
}