using ExchangeRates.Common.Caching;
using ExchangeRates.Common.Messaging.Handlers;
using ExchangeRates.Common.Messaging.Messages;
using ExchangeRates.Data;
using ExchangeRates.Data.Entities;
using ExchangeRates.Services.Currency.Dto;
using ExchangeRates.Services.Currency.Exceptions;
using ExchangeRates.Services.Currency.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExchangeRates.Services.Currency.Commands;

public struct CurrencyCreate(string code, string name) : ICommand<CurrencyDetailDto>
{
    public readonly string Code = code;
    public readonly string Name = name;
}

public class CurrencyCreateHandler(
    ExchangeRatesContext context,
    ILogger<CurrencyCreateHandler> logger,
    ICache cache)
    : ICommandHandler<CurrencyCreate, CurrencyDetailDto>
{
    public async Task<CurrencyDetailDto> Handle(CurrencyCreate command, CancellationToken ct = default)
    {
        if (await context.Currencies.AnyAsync(o => o.Code == command.Code, ct))
            throw new CurrencyAlreadyExists(command.Code);

        var currency = new CurrencyEntity
        {
            Code = command.Code,
            Name = command.Name
        };

        context.Add(currency);

        await context.SaveChangesAsync(ct);
        
        logger.LogInformation("Currency {CurrencyCode} created", command.Code);

        await cache.RemoveAsync(nameof(GetDefaultCurrency), ct);
            
        return new CurrencyDetailDto(currency);
    }
}