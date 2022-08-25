using ExchangeRates.Common.Messaging.Handlers;
using ExchangeRates.Common.Messaging.Messages;
using ExchangeRates.Data;
using ExchangeRates.Services.Currency.Dto;
using ExchangeRates.Services.Currency.Exceptions;
using ExchangeRates.Services.Currency.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExchangeRates.Services.Currency.Queries;

public class GetDefaultCurrency : IQuery<CurrencyDetailDto>
{
}

public class GetDefaultCurrencyHandler : IQueryHandler<GetDefaultCurrency, CurrencyDetailDto>
{
    private readonly ExchangeRatesContext _context;
    private readonly CurrencyOptions _currencyOptions;

    public GetDefaultCurrencyHandler(ExchangeRatesContext context, IOptions<CurrencyOptions> currencyOptions)
    {
        _context = context;
        _currencyOptions = currencyOptions.Value;
    }

    public async Task<CurrencyDetailDto> Handle(GetDefaultCurrency queru, CancellationToken ct = default)
    {
        var currency = await _context.Currencies.AsNoTracking()
            .Include(o => o.Rates).ThenInclude(o => o.FromCurrency)
            .FirstOrDefaultAsync(o => o.Code == _currencyOptions.DefaultCode, ct)
                ?? throw new CurrencyNotFound(_currencyOptions.DefaultCode);

        return new CurrencyDetailDto(currency);
    }
}