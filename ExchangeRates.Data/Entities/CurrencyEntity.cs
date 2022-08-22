using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeRates.Data.Entities;

[Table("Currencies")]
public class CurrencyEntity
{
    [Key]
    public int Id { get; set; }
    
    public string Code { get; set; }
    
    public bool IsDefault { get; set; }
    
    public virtual ICollection<CurrencyRateEntity> Rates { get; set; }
}