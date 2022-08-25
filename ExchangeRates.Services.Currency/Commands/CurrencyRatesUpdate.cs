using ExchangeRates.Common.Messaging;
using ExchangeRates.Common.Messaging.Handlers;
using ExchangeRates.Common.Messaging.Messages;
using ExchangeRates.Data;
using ExchangeRates.Data.Entities;
using ExchangeRates.Services.Currency.Dto;
using ExchangeRates.Services.Currency.Exceptions;
using ExchangeRates.Services.Currency.Providers;
using ExchangeRates.Services.Currency.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExchangeRates.Services.Currency.Commands;

public class CurrencyRatesUpdate : ICommand<CurrencyDetailDto>
{
    public CurrencyRatesUpdate(int id, string provider, 
        DateTime effectiveDate, IEnumerable<CurrencyRate> rates)
    {
        Id = id;
        Provider = provider;
        EffectiveDate = effectiveDate;
        Rates = rates;
    }

    public readonly int Id;
    public readonly string Provider;
    public readonly DateTime EffectiveDate;
    public readonly IEnumerable<CurrencyRate> Rates;
}

public class CurrencyRatesUpdateHandler : ICommandHandler<CurrencyRatesUpdate, CurrencyDetailDto>
{
    private readonly ExchangeRatesContext _context;
    private readonly ILogger<CurrencyRatesUpdateHandler> _logger;

    public CurrencyRatesUpdateHandler(ExchangeRatesContext context, ILogger<CurrencyRatesUpdateHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CurrencyDetailDto> Handle(CurrencyRatesUpdate command, CancellationToken ct = default)
    {
        var currencies = await _context.Currencies
            .Include(o => o.Rates).ThenInclude(o => o.FromCurrency)
            .ToListAsync(ct);

        var currency = currencies.SingleOrDefault(o => o.Id == command.Id)
            ?? throw new CurrencyNotFound(command.Id);

        currency.Provider = command.Provider;
        currency.EffectiveDate = command.EffectiveDate;
        
        var rates = new List<CurrencyRateEntity>(currency.Rates ?? new List<CurrencyRateEntity>());
        
        foreach (var rateModel in command.Rates)
        {
            if (rateModel.Rate <= 0)
                throw new InvalidCurrencyRate();
                
            var currencyRate = rates.SingleOrDefault(o => o.FromCurrency.Code == rateModel.Code);

            if (currencyRate != null)
            {
                currencyRate.Rate = rateModel.Rate;
                currencyRate.UpdatedAt = DateTime.Now;
            }
            else
                rates.Add(new CurrencyRateEntity
                {
                    FromCurrency = currencies.FirstOrDefault(o => o.Code == rateModel.Code) ?? new CurrencyEntity
                    {
                        Code = rateModel.Code,
                        Name = rateModel.Name
                    }, 
                    Rate = rateModel.Rate, 
                    UpdatedAt = DateTime.Now
                });
        }
        
        currency.Rates = rates;

        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Rates for {CurrencyCode} updated", currency.Code);

        return new CurrencyDetailDto(currency);
    }
}