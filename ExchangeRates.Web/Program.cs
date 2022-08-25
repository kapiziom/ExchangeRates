using ExchangeRates.Common.Caching;
using ExchangeRates.Common.Messaging;
using ExchangeRates.Data;
using ExchangeRates.Providers.Nbp;
using ExchangeRates.Services.Currency.Options;
using ExchangeRates.Services.Currency.Providers;
using ExchangeRates.Services.Currency.Queries;
using ExchangeRates.Web.BackgroundServices;
using ExchangeRates.Web.Infrastructure.Caching;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddScoped<ICache, MemoryCache>();

builder.Services.AddHttpClient();

builder.Services.AddDbContext<ExchangeRatesContext>(options =>
{
    options.UseInMemoryDatabase("InMemory");
});

builder.Services.AddOptions<CurrencyOptions>().Bind(builder.Configuration.GetRequiredSection("Currencies"));

builder.Services.AddControllersWithViews();
            
builder.Services.AddScoped<ICurrencyProvider, NbpProvider>();

builder.Services.AddScoped<IMessageBroker, ExchangeRates.Web.Infrastructure.Messaging.MediatR>();

builder.Services.AddMediatR(
    typeof(Program).Assembly,
    typeof(GetDefaultCurrency).Assembly);

builder.Services.AddHostedService<CurrencyRatesService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Currency/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Currency}/{action=Index}/{id?}");

app.Run();