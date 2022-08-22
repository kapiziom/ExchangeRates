using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeRates.Data.Entities;

[Table("CurrencyRates")]
public class CurrencyRateEntity
{
    public int BaseId { get; set; }
    
    [ForeignKey(nameof(BaseId))]
    public CurrencyEntity BaseCurrency { get; set; }
    
    public int CompareId { get; set; }
    
    [ForeignKey(nameof(CompareId))]
    public CurrencyEntity CompareCurrency { get; set; }

    public decimal Rate { get; set; }
}