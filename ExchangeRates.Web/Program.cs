using ExchangeRates.Common.Messaging;
using ExchangeRates.Data;
using ExchangeRates.Providers.Nbp;
using ExchangeRates.Services.Currency.Providers;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddDbContext<ExchangeRatesContext>(options =>
{
    options.UseInMemoryDatabase("InMemory");
});

builder.Services.AddControllersWithViews();
            
builder.Services.AddScoped<ICurrencyProvider, NbpProvider>();

builder.Services.AddScoped<IMessageBroker, ExchangeRates.Web.Infrastructure.Messaging.MediatR>();

builder.Services.AddMediatR(typeof(Program).Assembly);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();