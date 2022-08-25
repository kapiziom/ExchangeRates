using ExchangeRates.Common.Exceptions;

namespace ExchangeRates.Services.Currency.Exceptions;

public class CurrencyAlreadyExists : ServiceException
{
    public CurrencyAlreadyExists(string code) : base($"Currency with code {code} already exists.")
    {
    }
}