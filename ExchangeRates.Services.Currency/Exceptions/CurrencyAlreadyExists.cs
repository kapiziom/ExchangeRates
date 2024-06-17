using ExchangeRates.Common.Exceptions;

namespace ExchangeRates.Services.Currency.Exceptions;

public class CurrencyAlreadyExists(string code) : ServiceException($"Currency with code {code} already exists.");