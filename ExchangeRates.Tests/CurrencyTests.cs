using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRates.Common.Messaging;
using ExchangeRates.Data;
using ExchangeRates.Data.Entities;
using ExchangeRates.Services.Currency.Commands;
using ExchangeRates.Services.Currency.Exceptions;
using ExchangeRates.Services.Currency.Options;
using ExchangeRates.Services.Currency.Providers;
using ExchangeRates.Services.Currency.Queries;
using ExchangeRates.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ExchangeRates.Tests;

public class CurrencyTests
{
    [Fact]
    public async Task CreateCurrency_CorrectAdd_Ok()
    {
        // Arrange
        await using var application = new TestApplication();

        await using var scope = application.Services.CreateAsyncScope();
        
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();

        var currencyCode = "XYZ";
        var currencyName = "test_currency";
        
        // Act
        var currency = await messageBroker.SendCommandAsync(new CurrencyCreate(currencyCode, currencyName));

        // Assert
        Assert.Equal(currencyCode, currency.Code);
        Assert.Equal(currencyName, currency.Name);
    }
    
    [Fact]
    public async Task CreateCurrency_TryAddDuplicate_ThrowsCurrencyAlreadyExists()
    {
        // Arrange
        await using var application = new TestApplication();

        await using var scope = application.Services.CreateAsyncScope();
        
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();

        var currencyCode = "ABC";
        var currencyName = "test_currency";
        
        // Act
        await messageBroker.SendCommandAsync(new CurrencyCreate(currencyCode, currencyName));

        // Assert
        await Assert.ThrowsAsync<CurrencyAlreadyExists>(async () =>
            await messageBroker.SendCommandAsync(new CurrencyCreate(currencyCode, currencyName)));
    }
    
    [Fact]
    public async Task UpdateCurrencyRates_UsingProviderData_Ok()
    {
        // Arrange
        await using var application = new TestApplication();

        await using var scope = application.Services.CreateAsyncScope();
        
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();
        var context = scope.ServiceProvider.GetRequiredService<ExchangeRatesContext>();
        var currencyOptions = scope.ServiceProvider.GetRequiredService<IOptions<CurrencyOptions>>();
        
        var currencyProvider = new Mock<ICurrencyProvider>();
        currencyProvider.Setup(x => x.GetRatesAsync(currencyOptions.Value.DefaultCode, default))
            .ReturnsAsync(() => new CurrencyProviderDto
            {
                Provider = "NBP",
                Description = "TEST",
                EffectiveDate = DateTime.Today,
                Rates = new List<CurrencyRate>
                {
                    new (){Code = "USD", Name = "dollar", Rate = 4.75m},
                    new (){Code = "EUR", Name = "euro", Rate = 4.75m},
                    new (){Code = "CHF", Name = "frank", Rate = 4.94m}
                }
            });

        var defaultCurrency = await GetDefaultCurrency(context, currencyOptions.Value);
        
        var providerModel = await currencyProvider.Object.GetRatesAsync(currencyOptions.Value.DefaultCode);

        // Act
        await messageBroker.SendCommandAsync(new CurrencyRatesUpdate(defaultCurrency.Id,
            providerModel.Provider, providerModel.EffectiveDate, providerModel.Rates));
        
        var defaultCurrencyDto = await messageBroker.SendQueryAsync(new GetDefaultCurrency(string.Empty, string.Empty));

        // Assert
        Assert.Equal(currencyOptions.Value.DefaultCode, defaultCurrencyDto.Code);
        Assert.Equal(providerModel.Rates.Count(), defaultCurrencyDto.Rates.Count());
        Assert.Equal(
            providerModel.Rates.Select(o => new { Code = o.Code, Name = o.Name, Rate = o.Rate}).OrderBy(o => o.Code),
            defaultCurrencyDto.Rates.Select(o => new { Code = o.Symbol, Name = o.Name, Rate = o.Rate}).OrderBy(o => o.Code));
    }

    [Fact]
    public async Task UpdateCurrencyRates_NotExistingId_ThrowsCurrencyNotFound()
    {
        // Arrange
        await using var application = new TestApplication();

        await using var scope = application.Services.CreateAsyncScope();
        
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();
        
        var command = new CurrencyRatesUpdate(999999, "TEST", DateTime.Now, new List<CurrencyRate>());

        // Assert
        await Assert.ThrowsAsync<CurrencyNotFound>(async () =>
            await messageBroker.SendCommandAsync(command));
    }

    private async Task<CurrencyEntity> GetDefaultCurrency(ExchangeRatesContext context, CurrencyOptions currencyOptions)
    {
        var defaultCurrency = await context.Currencies
            .SingleOrDefaultAsync(o => o.Code == currencyOptions.DefaultCode);

        if (defaultCurrency is not null)
            return defaultCurrency;
        
        defaultCurrency = new CurrencyEntity
        {
            Code = currencyOptions.DefaultCode,
            Name = currencyOptions.DefaultName
        };

        context.Add(defaultCurrency);

        await context.SaveChangesAsync();

        return defaultCurrency;
    }
}