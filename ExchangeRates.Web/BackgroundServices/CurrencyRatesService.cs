using ExchangeRates.Common.Messaging;
using ExchangeRates.Data;
using ExchangeRates.Data.Entities;
using ExchangeRates.Services.Currency.Commands;
using ExchangeRates.Services.Currency.Options;
using ExchangeRates.Services.Currency.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExchangeRates.Web.BackgroundServices;

public class CurrencyRatesService : BackgroundService
{
    private const int PollIntervalInSeconds = 1800;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CurrencyRatesService> _logger;
    private readonly CurrencyOptions _currencyOptions;
    
    public CurrencyRatesService(IServiceScopeFactory scopeFactory,
        ILogger<CurrencyRatesService> logger, IOptions<CurrencyOptions> currencyOptions)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _currencyOptions = currencyOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                
                var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();
                
                var currencyProvider = scope.ServiceProvider.GetRequiredService<ICurrencyProvider>();
                
                var context = scope.ServiceProvider.GetRequiredService<ExchangeRatesContext>();

                var defaultCurrency = await context.Currencies
                    .SingleOrDefaultAsync(o => o.Code == _currencyOptions.DefaultCode, ct);
                
                if (defaultCurrency is null)
                {
                    defaultCurrency = new CurrencyEntity
                    {
                        Code = _currencyOptions.DefaultCode,
                        Name = _currencyOptions.DefaultName
                    };

                    context.Add(defaultCurrency);

                    await context.SaveChangesAsync(ct);
                }

                var providerModel = await currencyProvider.GetRatesAsync(defaultCurrency.Code, ct);

                await messageBroker.SendCommandAsync(new CurrencyRatesUpdate(defaultCurrency.Id,
                    providerModel.Provider, providerModel.EffectiveDate, providerModel.Rates), ct);

                context.ChangeTracker.Clear();
                
                _logger.LogInformation("CurrencyRatesService processed the rates");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to process currencies. {ExceptionMessage}", ex.Message);
            }
            
            await Task.Delay(1000 * PollIntervalInSeconds, ct);
        }
    }
}