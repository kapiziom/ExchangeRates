using ExchangeRates.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRates.Data;

public class ExchangeRatesContext : DbContext
{
    public ExchangeRatesContext(DbContextOptions<ExchangeRatesContext> options) : base(options)
    {
    }
    
    public DbSet<CurrencyEntity> Currencies { get; set; }
    public DbSet<CurrencyRateEntity> CurrencyRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CurrencyEntity>()
            .HasIndex(o => o.Code)
            .IsUnique();
        
        modelBuilder.Entity<CurrencyRateEntity>()
            .HasKey(o => new { o.BaseId, o.CompareId });
    }
}