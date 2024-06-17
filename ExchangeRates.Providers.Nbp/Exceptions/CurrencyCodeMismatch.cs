using ExchangeRates.Common.Exceptions;

namespace ExchangeRates.Providers.Nbp.Exceptions;

public class CurrencyCodeMismatch(string code)
    : ServiceException($"Currency code {code} mismatch with provider base currency.");