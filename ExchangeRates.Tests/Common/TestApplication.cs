using System.Linq;
using ExchangeRates.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ExchangeRates.Tests.Common;

internal class TestApplication : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
            
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ExchangeRatesContext>));

            services.Remove(descriptor);

            services.AddDbContext<ExchangeRatesContext>(options =>
            {
                options.UseInMemoryDatabase("Tests");
            });
            
            services.RemoveAll(typeof(IHostedService));
        });
            
        builder.UseTestServer();
    }
}