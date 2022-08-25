using ExchangeRates.Common.Exceptions;

namespace ExchangeRates.Providers.Nbp.Exceptions;

public class CurrencyCodeMismatch : ServiceException
{
    public CurrencyCodeMismatch(string code) : base($"Currency code {code} mismatch with provider base currency.")
    {
    }
}