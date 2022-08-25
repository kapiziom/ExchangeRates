using System.Diagnostics.CodeAnalysis;
using ExchangeRates.Common.Caching;
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
    private readonly ICache _cache;

    public GetDefaultCurrencyHandler(ExchangeRatesContext context, IOptions<CurrencyOptions> currencyOptions, ICache cache)
    {
        _context = context;
        _currencyOptions = currencyOptions.Value;
        _cache = cache;
    }

    public async Task<CurrencyDetailDto> Handle(GetDefaultCurrency query, CancellationToken ct = default)
    {
        var currencyDetailDto = await _cache.GetAsync<CurrencyDetailDto>(nameof(GetDefaultCurrency), ct);

        if (currencyDetailDto is null)
        {
            var currency = await _context.Currencies.AsNoTracking()
                .Include(o => o.Rates).ThenInclude(o => o.FromCurrency)
                .FirstOrDefaultAsync(o => o.Code == _currencyOptions.DefaultCode, ct) 
                    ?? throw new CurrencyNotFound(_currencyOptions.DefaultCode);

            currencyDetailDto = new CurrencyDetailDto(currency);
            
            await _cache.SetAsync(nameof(GetDefaultCurrency), currencyDetailDto, new CacheEntryOptions
                { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }, ct);
        }

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
                _ => currencyDetailDto.Rates
            };
        }
        
        return currencyDetailDto;
    }
}