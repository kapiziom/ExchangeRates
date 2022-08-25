using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeRates.Data.Entities;

[Table("CurrencyRates")]
public class CurrencyRateEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int BaseCurrencyId { get; set; }
    public CurrencyEntity BaseCurrency { get; set; }
    
    public int FromCurrencyId { get; set; }
    public CurrencyEntity FromCurrency { get; set; }

    public decimal Rate { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}