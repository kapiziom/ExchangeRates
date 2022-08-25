using ExchangeRates.Common.Exceptions;

namespace ExchangeRates.Services.Currency.Exceptions;

public class InvalidCurrencyRate : ServiceException
{
    public InvalidCurrencyRate() : base("Invalid currency rate.")
    {
    }
}