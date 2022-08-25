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

public class CurrencyCreate : ICommand<CurrencyDetailDto>
{
    public CurrencyCreate(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public readonly string Code;
    public readonly string Name;
}

public class CurrencyCreateHandler : ICommandHandler<CurrencyCreate, CurrencyDetailDto>
{
    private readonly ExchangeRatesContext _context;
    private readonly ILogger<CurrencyCreateHandler> _logger;
    private readonly ICache _cache;

    public CurrencyCreateHandler(ExchangeRatesContext context, ILogger<CurrencyCreateHandler> logger, ICache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<CurrencyDetailDto> Handle(CurrencyCreate command, CancellationToken ct = default)
    {
        if (await _context.Currencies.AnyAsync(o => o.Code == command.Code, ct))
            throw new CurrencyAlreadyExists(command.Code);

        var currency = new CurrencyEntity
        {
            Code = command.Code,
            Name = command.Name
        };

        _context.Add(currency);

        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Currency {CurrencyCode} created", command.Code);

        await _cache.RemoveAsync(nameof(GetDefaultCurrency), ct);
            
        return new CurrencyDetailDto(currency);
    }
}