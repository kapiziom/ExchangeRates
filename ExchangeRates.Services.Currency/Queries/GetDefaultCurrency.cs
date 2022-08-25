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
    public GetDefaultCurrency(string sortBy, string sortOrder)
    {
        SortBy = sortBy;
        SortOrder = sortOrder;
    }
    
    public readonly string SortBy;
    public readonly string SortOrder;
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

    public async Task<CurrencyDetailDto> Handle(GetDefaultCurrency query, CancellationToken ct = default)
    {
        var currency = await _context.Currencies.AsNoTracking()
            .Include(o => o.Rates).ThenInclude(o => o.FromCurrency)
            .FirstOrDefaultAsync(o => o.Code == _currencyOptions.DefaultCode, ct)
                ?? throw new CurrencyNotFound(_currencyOptions.DefaultCode);

        var currencyDetailDto = new CurrencyDetailDto(currency);

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) ? "asc" : query.SortOrder;
            
            currencyDetailDto.Rates = query.SortBy switch
            {
                "name" => sortOrder is "asc"
                    ? currencyDetailDto.Rates.OrderBy(o => o.Name)
                    : currencyDetailDto.Rates.OrderByDescending(o => o.Name),
                "code"  => sortOrder is "asc"
                    ? currencyDetailDto.Rates.OrderBy(o => o.Symbol)
                    : currencyDetailDto.Rates.OrderByDescending(o => o.Symbol),
                "rate"  => sortOrder is "asc"
                    ? currencyDetailDto.Rates.OrderBy(o => o.Rate)
                    : currencyDetailDto.Rates.OrderByDescending(o => o.Rate),
            };
        }
        
        return currencyDetailDto;
    }
}