using ExchangeRates.Common.Exceptions;

namespace ExchangeRates.Services.Currency.Exceptions;

public class CurrencyNotFound : ServiceException
{
    public CurrencyNotFound(string code) : base($"Currency {code} not found.", ExceptionEnum.NotFound)
    {
    }
    
    public CurrencyNotFound(int id) : base($"Currency {id} not found.", ExceptionEnum.NotFound)
    {
    }
}