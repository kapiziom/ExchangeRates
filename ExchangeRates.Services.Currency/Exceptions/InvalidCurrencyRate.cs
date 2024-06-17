using ExchangeRates.Common.Exceptions;

namespace ExchangeRates.Services.Currency.Exceptions;

public class InvalidCurrencyRate() : ServiceException("Invalid currency rate.");