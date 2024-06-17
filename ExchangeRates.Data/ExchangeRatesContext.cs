using ExchangeRates.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRates.Data;

public class ExchangeRatesContext(DbContextOptions<ExchangeRatesContext> options) : DbContext(options)
{
    public DbSet<CurrencyEntity> Currencies { get; set; }
    public DbSet<CurrencyRateEntity> CurrencyRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CurrencyEntity>()
            .HasIndex(o => o.Code)
            .IsUnique();

        modelBuilder.Entity<CurrencyEntity>()
            .Property(o => o.Name)
            .IsRequired(false);

        modelBuilder.Entity<CurrencyEntity>()
            .HasMany(o => o.Rates)
            .WithOne(o => o.BaseCurrency)
            .HasForeignKey(o => o.BaseCurrencyId);

        modelBuilder.Entity<CurrencyEntity>()
            .HasMany(o => o.FromRates)
            .WithOne(o => o.FromCurrency)
            .HasForeignKey(o => o.FromCurrencyId);
        
        modelBuilder.Entity<CurrencyRateEntity>()
            .HasIndex(o => new { o.BaseCurrencyId, o.FromCurrencyId })
            .IsUnique();
    }
}