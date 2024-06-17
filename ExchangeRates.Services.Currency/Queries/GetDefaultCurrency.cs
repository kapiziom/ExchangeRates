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

public struct GetDefaultCurrency(string sortBy, string sortOrder) : IQuery<CurrencyDetailDto>
{
    public readonly string SortBy = sortBy;
    public readonly string SortOrder = sortOrder;
}

public class GetDefaultCurrencyHandler(
    ExchangeRatesContext context,
    IOptions<CurrencyOptions> currencyOptions,
    ICache cache)
    : IQueryHandler<GetDefaultCurrency, CurrencyDetailDto>
{
    private readonly CurrencyOptions _currencyOptions = currencyOptions.Value;

    public async Task<CurrencyDetailDto> Handle(GetDefaultCurrency query, CancellationToken ct = default)
    {
        var currencyDetailDto = await cache.GetAsync<CurrencyDetailDto>(nameof(GetDefaultCurrency), ct);

        if (currencyDetailDto is null)
        {
            var currency = await context.Currencies.AsNoTracking()
                .Include(o => o.Rates).ThenInclude(o => o.FromCurrency)
                .FirstOrDefaultAsync(o => o.Code == _currencyOptions.DefaultCode, ct) 
                    ?? throw new CurrencyNotFound(_currencyOptions.DefaultCode);

            currencyDetailDto = new CurrencyDetailDto(currency);
            
            await cache.SetAsync(nameof(GetDefaultCurrency), currencyDetailDto, new CacheEntryOptions
                { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }, ct);
        }

        if (string.IsNullOrEmpty(query.SortBy))
            return currencyDetailDto;
        
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

        return currencyDetailDto;
    }
}